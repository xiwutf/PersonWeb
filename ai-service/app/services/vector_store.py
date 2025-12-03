"""
向量存储服务
封装 Chroma 向量数据库的操作，提供统一的向量存储和检索接口
"""

import os
from typing import List, Dict, Any, Optional
from app.core.config import settings
from app.core.app_logging import logger


class VectorStore:
    """向量存储服务类"""
    
    def __init__(self):
        """初始化向量存储服务"""
        # 如果路径是相对路径，转换为绝对路径（基于工作目录）
        db_path = settings.CHROMA_DB_PATH
        if not os.path.isabs(db_path):
            # 获取工作目录（通常是 /srv/ai-service）
            work_dir = os.getcwd()
            db_path = os.path.join(work_dir, db_path)
        
        self.db_path = db_path
        self.collection_name = settings.CHROMA_COLLECTION_NAME
        self._client = None
        self._collection = None
        
        # 确保数据目录存在（带错误处理）
        try:
            os.makedirs(self.db_path, exist_ok=True)
            logger.info(f"向量数据库目录已准备: {self.db_path}")
        except PermissionError as e:
            logger.error(f"无法创建向量数据库目录 {self.db_path}: {e}")
            logger.error("请确保目录存在且服务用户有写入权限")
            raise
        except Exception as e:
            logger.error(f"创建向量数据库目录失败: {e}")
            raise
    
    def _get_client(self):
        """获取 Chroma 客户端（延迟初始化）"""
        if self._client is None:
            try:
                import chromadb
                from chromadb.config import Settings as ChromaSettings
                
                # 创建持久化客户端
                self._client = chromadb.PersistentClient(
                    path=self.db_path,
                    settings=ChromaSettings(
                        anonymized_telemetry=False,
                        allow_reset=True
                    )
                )
                logger.info(f"Chroma 客户端初始化成功: path={self.db_path}")
            except ImportError:
                error_msg = "chromadb 库未安装，请运行: pip install chromadb"
                logger.error(error_msg)
                raise ImportError(error_msg)
            except Exception as e:
                logger.error(f"初始化 Chroma 客户端失败: {str(e)}", exc_info=True)
                raise
        
        return self._client
    
    def _get_collection(self, document_id: Optional[str] = None):
        """
        获取或创建集合
        
        Args:
            document_id: 文档 ID（可选，如果提供则使用文档特定的集合）
            
        Returns:
            Collection: Chroma 集合对象
        """
        client = self._get_client()
        
        # 如果提供了 document_id，使用文档特定的集合名称
        # 否则使用默认集合
        # 确保 document_id 转换为字符串，避免类型不一致
        if document_id:
            collection_name = f"{self.collection_name}_{str(document_id)}"
        else:
            collection_name = self.collection_name
        
        try:
            # 尝试获取现有集合
            collection = client.get_collection(name=collection_name)
            logger.debug(f"获取现有集合: {collection_name}")
        except Exception:
            # 如果集合不存在，创建新集合
            collection = client.create_collection(
                name=collection_name,
                metadata={"description": f"文档向量集合: {document_id or 'default'}"}
            )
            logger.info(f"创建新集合: {collection_name}")
        
        return collection
    
    async def add_vectors(
        self,
        vectors: List[List[float]],
        texts: List[str],
        metadata: List[Dict[str, Any]],
        document_id: Optional[str] = None
    ) -> List[str]:
        """
        添加向量到向量数据库
        
        Args:
            vectors: 向量列表
            texts: 文本列表（与向量一一对应）
            metadata: 元数据列表（与向量一一对应）
            document_id: 文档 ID（可选，用于组织集合）
            
        Returns:
            List[str]: 存储后的向量 ID 列表
        """
        if not vectors or not texts or not metadata:
            raise ValueError("vectors、texts 和 metadata 不能为空")
        
        if len(vectors) != len(texts) or len(vectors) != len(metadata):
            raise ValueError("vectors、texts 和 metadata 的长度必须一致")
        
        try:
            # 确保 document_id 是字符串类型
            if document_id is not None:
                document_id = str(document_id)
            
            collection = self._get_collection(document_id)
            
            # 生成向量 ID（如果 metadata 中没有提供 id）
            vector_ids = []
            for i, meta in enumerate(metadata):
                if "id" in meta:
                    vector_id = str(meta["id"])
                else:
                    # 生成唯一 ID
                    chunk_id = meta.get("chunk_id") or meta.get("chunk_index", i)
                    vector_id = f"{document_id or 'doc'}_{chunk_id}" if document_id else f"chunk_{chunk_id}"
                vector_ids.append(vector_id)
            
            # 准备元数据（Chroma 要求元数据值必须是字符串、数字或布尔值）
            chroma_metadata = []
            for meta in metadata:
                chroma_meta = {}
                for key, value in meta.items():
                    # 转换元数据值类型
                    if isinstance(value, (str, int, float, bool)):
                        # 确保 document_id 在元数据中也是字符串类型
                        if key == "document_id":
                            chroma_meta[key] = str(value)
                        else:
                            chroma_meta[key] = value
                    elif value is None:
                        continue  # 跳过 None 值
                    else:
                        # 其他类型转换为字符串
                        chroma_meta[key] = str(value)
                chroma_metadata.append(chroma_meta)
            
            # 添加到集合
            collection.add(
                ids=vector_ids,
                embeddings=vectors,
                documents=texts,
                metadatas=chroma_metadata
            )
            
            logger.info(f"向量存储成功: count={len(vector_ids)}, document_id={document_id}")
            
            return vector_ids
            
        except Exception as e:
            logger.error(f"存储向量失败: {str(e)}", exc_info=True)
            raise
    
    async def search_vectors(
        self,
        query_vector: List[float],
        top_k: int = 5,
        document_id: Optional[str] = None,
        filter_metadata: Optional[Dict[str, Any]] = None
    ) -> List[Dict[str, Any]]:
        """
        在向量数据库中检索相似向量
        
        Args:
            query_vector: 查询向量
            top_k: 返回数量
            document_id: 文档 ID（可选，限定文档）
            filter_metadata: 元数据过滤条件（可选）
            
        Returns:
            List[Dict[str, Any]]: 检索结果列表，每个结果包含：
                - id: 向量 ID
                - distance: 距离（越小越相似）
                - score: 相似度分数（0-1，越大越相似）
                - document: 文档文本
                - metadata: 元数据
        """
        try:
            # 确保 document_id 是字符串类型
            if document_id is not None:
                document_id = str(document_id)
            
            collection = self._get_collection(document_id)
            collection_name = f"{self.collection_name}_{document_id}" if document_id else self.collection_name
            logger.info(f"使用集合: {collection_name}, document_id={document_id}")
            
            # 先检查集合中是否有数据
            try:
                collection_count = collection.count()
                logger.info(f"集合中的向量数量: {collection_count}")
                if collection_count == 0:
                    logger.warning(f"⚠️ 集合 {collection_name} 中没有向量数据！可能原因：1) 文档尚未处理完成 2) 文档处理失败 3) document_id 不匹配")
            except Exception as e:
                logger.warning(f"无法获取集合数量: {str(e)}")
            
            # 构建查询条件
            where = filter_metadata or {}
            if document_id and "document_id" not in where:
                where["document_id"] = str(document_id)  # 确保是字符串类型
            
            logger.debug(f"查询条件: where={where}, top_k={top_k}")
            
            # 执行查询
            results = collection.query(
                query_embeddings=[query_vector],
                n_results=top_k,
                where=where if where else None
            )
            
            logger.debug(f"查询结果: ids_count={len(results.get('ids', [[]])[0]) if results.get('ids') else 0}")
            
            # 解析结果
            result_list = []
            if results["ids"] and len(results["ids"][0]) > 0:
                # 获取所有距离值，用于归一化
                distances = results["distances"][0] if results.get("distances") else []
                max_distance = max(distances) if distances else 1.0
                min_distance = min(distances) if distances else 0.0
                distance_range = max_distance - min_distance if max_distance > min_distance else 1.0
                
                for i in range(len(results["ids"][0])):
                    distance = results["distances"][0][i] if results.get("distances") and i < len(results["distances"][0]) else 0.0
                    
                    # 将距离转换为相似度分数（0-1）
                    # ChromaDB 可能返回负数距离，需要正确处理
                    if distance_range > 0:
                        # 归一化：score = 1 - (distance - min_distance) / range
                        # 这样距离最小的得分为 1，距离最大的得分为 0
                        # 注意：如果距离是负数，min_distance 也是负数，归一化仍然有效
                        normalized_score = 1.0 - ((distance - min_distance) / distance_range)
                    else:
                        # 如果所有距离相同，给相同的分数
                        normalized_score = 0.5
                    
                    # 确保 score 在 0-1 范围内（强制限制）
                    score = max(0.0, min(1.0, normalized_score))
                    
                    # 调试日志：如果距离异常，记录日志
                    if distance < -100 or distance > 100:
                        logger.warning(f"检测到异常距离值: distance={distance}, 归一化后 score={normalized_score}, 最终 score={score}")
                    
                    result_item = {
                        "id": results["ids"][0][i],
                        "distance": distance,
                        "score": score,  # 确保在 0-1 范围内
                        "document": results["documents"][0][i] if results.get("documents") and i < len(results["documents"][0]) else "",
                        "metadata": results["metadatas"][0][i] if results.get("metadatas") and i < len(results["metadatas"][0]) else {}
                    }
                    result_list.append(result_item)
            
            logger.info(f"向量检索完成: top_k={top_k}, results_count={len(result_list)}, document_id={document_id}")
            
            return result_list
            
        except Exception as e:
            logger.error(f"向量检索失败: {str(e)}", exc_info=True)
            raise
    
    async def delete_vectors(
        self,
        vector_ids: List[str],
        document_id: Optional[str] = None
    ) -> bool:
        """
        删除向量
        
        Args:
            vector_ids: 要删除的向量 ID 列表
            document_id: 文档 ID（可选）
            
        Returns:
            bool: 是否成功
        """
        try:
            collection = self._get_collection(document_id)
            collection.delete(ids=vector_ids)
            
            logger.info(f"删除向量成功: count={len(vector_ids)}, document_id={document_id}")
            
            return True
            
        except Exception as e:
            logger.error(f"删除向量失败: {str(e)}", exc_info=True)
            raise
    
    async def delete_document_vectors(
        self,
        document_id: str
    ) -> bool:
        """
        删除文档的所有向量
        
        Args:
            document_id: 文档 ID
            
        Returns:
            bool: 是否成功
        """
        try:
            collection = self._get_collection(document_id)
            
            # 查询该文档的所有向量
            all_results = collection.get(
                where={"document_id": document_id}
            )
            
            if all_results["ids"]:
                # 删除所有向量
                collection.delete(ids=all_results["ids"])
                logger.info(f"删除文档所有向量成功: document_id={document_id}, count={len(all_results['ids'])}")
            else:
                logger.info(f"文档没有向量: document_id={document_id}")
            
            return True
            
        except Exception as e:
            logger.error(f"删除文档向量失败: {str(e)}", exc_info=True)
            raise


# 创建全局向量存储实例
vector_store = VectorStore()

