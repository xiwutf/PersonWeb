"""
文档知识管家 Agent 服务
处理文档解析、分段、摘要、知识结构构建、向量化、问答等核心功能
"""

from typing import List, Dict, Any, Optional
from app.core.app_logging import logger
from app.models.dto import (
    DocumentProcessResponseData,
    DocumentChunkData,
    DocumentQueryResponseData,
    RelevantChunk
)
from app.services.document_tools import DocumentTools


class DocumentAgentService:
    """文档知识管家 Agent 服务类"""
    
    def __init__(self):
        """初始化服务，创建工具实例"""
        self.tools = DocumentTools()
    
    async def process_document(
        self,
        document_id: str,
        file_path: str,
        file_type: str,
        user_id: str
    ) -> DocumentProcessResponseData:
        """
        处理文档（完整流程）
        
        1. 解析文档提取文本
        2. 文本分段
        3. 生成摘要
        4. 构建知识结构
        5. 向量化并存储
        
        Args:
            document_id: 文档 ID
            file_path: 文件路径
            file_type: 文件类型
            user_id: 用户 ID
            
        Returns:
            DocumentProcessResponseData: 处理结果
        """
        try:
            logger.info(f"开始处理文档: document_id={document_id}, file_type={file_type}")
            
            # 1. 解析文档
            logger.info("步骤 1/5: 解析文档...")
            parse_result = await self.tools.parse_document(file_path, file_type)
            raw_text = parse_result["raw_text"]
            metadata = parse_result.get("metadata", {})
            
            logger.info(f"文档解析完成: text_length={len(raw_text)}, metadata={metadata}")
            
            # 2. 文本分段
            logger.info("步骤 2/5: 文本分段...")
            chunk_result = await self.tools.chunk_text(
                text=raw_text,
                chunk_size=1000,  # 每段约 1000 字符
                chunk_overlap=200  # 重叠 200 字符
            )
            chunks = chunk_result["chunks"]
            chunk_metadata = chunk_result.get("chunk_metadata", [])
            
            logger.info(f"文本分段完成: chunks_count={len(chunks)}")
            
            # 3. 生成文档摘要
            logger.info("步骤 3/5: 生成文档摘要...")
            summary_result = await self.tools.generate_summary(
                text=raw_text,
                summary_type="document"
            )
            document_summary = summary_result["summary"]
            
            logger.info(f"文档摘要生成完成: summary_length={len(document_summary)}")
            
            # 4. 构建知识结构
            logger.info("步骤 4/5: 构建知识结构...")
            knowledge_structure = await self.tools.build_knowledge_structure(
                chunks=chunks,
                document_title=metadata.get("title", "未命名文档")
            )
            
            logger.info("知识结构构建完成")
            
            # 5. 向量化并存储
            logger.info("步骤 5/5: 向量化并存储...")
            chunk_data_list = []
            for i, chunk in enumerate(chunks):
                # 生成分段摘要（可选）
                chunk_summary = None
                try:
                    chunk_summary_result = await self.tools.generate_summary(
                        text=chunk,
                        summary_type="chunk"
                    )
                    chunk_summary = chunk_summary_result["summary"]
                except Exception as e:
                    logger.warning(f"生成分段摘要失败 (chunk {i}): {str(e)}")
                
                # 向量化
                embed_result = await self.tools.embed_text(
                    text=chunk,
                    model_name="text-embedding-ada-002"  # 默认使用 OpenAI embedding 模型
                )
                vector = embed_result["vector"]
                
                # 存储向量
                store_result = await self.tools.store_vectors(
                    vectors=[vector],
                    metadata=[{
                        "document_id": document_id,
                        "chunk_id": i,  # chunk_id 用于标识分段
                        "chunk_index": i,  # chunk_index 用于排序
                        "content": chunk,  # 存储完整内容（Chroma 会存储文档文本）
                        "summary": chunk_summary or ""  # 分段摘要
                    }]
                )
                vector_id = store_result.get("vector_ids", [None])[0]
                
                chunk_data = DocumentChunkData(
                    index=i,
                    content=chunk,
                    summary=chunk_summary,
                    metadata=chunk_metadata[i] if i < len(chunk_metadata) else None,
                    vector_id=vector_id
                )
                chunk_data_list.append(chunk_data)
            
            logger.info(f"向量化存储完成: chunks_count={len(chunk_data_list)}")
            
            # 返回处理结果
            return DocumentProcessResponseData(
                document_id=document_id,
                summary=document_summary,
                knowledge_structure=knowledge_structure,
                total_chunks=len(chunk_data_list),
                chunks=chunk_data_list
            )
            
        except Exception as e:
            logger.error(f"处理文档失败: {str(e)}", exc_info=True)
            raise
    
    async def query_document(
        self,
        document_id: str,
        user_id: str,
        query: str,
        top_k: int = 5
    ) -> DocumentQueryResponseData:
        """
        文档问答
        
        1. 将问题向量化
        2. 在向量数据库中检索相关文档片段
        3. 使用 LLM 生成答案
        
        Args:
            document_id: 文档 ID
            user_id: 用户 ID
            query: 用户问题
            top_k: 返回相关文档片段数量
            
        Returns:
            DocumentQueryResponseData: 包含答案和相关文档片段
        """
        try:
            print(f"\n{'='*60}")
            print(f"🔍 开始文档问答")
            print(f"document_id={document_id}, query={query[:50]}..., top_k={top_k}")
            print(f"{'='*60}\n")
            
            logger.info(f"开始文档问答: document_id={document_id}, query={query[:50]}...")
            
            # 1. 将问题向量化
            print("📊 步骤 1/3: 将问题向量化...")
            logger.info("步骤 1/3: 将问题向量化")
            embed_result = await self.tools.embed_text(
                text=query,
                model_name="text-embedding-ada-002"
            )
            query_vector = embed_result["vector"]
            print(f"✅ 向量化完成: vector_length={len(query_vector)}")
            
            # 2. 在向量数据库中检索
            print(f"🔍 步骤 2/3: 在向量数据库中检索 (top_k={top_k})...")
            logger.info(f"步骤 2/3: 在向量数据库中检索")
            # 确保 document_id 是字符串类型
            document_id_str = str(document_id) if document_id else None
            print(f"📋 检索参数: document_id={document_id_str} (类型: {type(document_id_str).__name__})")
            search_result = await self.tools.search_vectors(
                query_vector=query_vector,
                top_k=top_k,
                document_id=document_id_str  # 限定在当前文档
            )
            results = search_result["results"]
            
            print(f"✅ 向量检索完成: results_count={len(results)}")
            logger.info(f"向量检索完成: results_count={len(results)}")
            
            # 3. 使用 LLM 生成答案
            print(f"🤖 步骤 3/3: 使用 LLM 生成答案...")
            logger.info(f"步骤 3/3: 使用 LLM 生成答案")
            
            # 构建上下文（相关文档片段）
            # 格式：每个片段包含索引、内容和摘要（如果有）
            context_parts = []
            for i, r in enumerate(results):
                chunk_index = r.get("chunk_index", i)
                content = r.get("content", "")
                summary = r.get("summary", "")
                
                # 构建片段文本
                chunk_text = f"[片段 {chunk_index}]: {content}"
                if summary:
                    chunk_text += f"\n片段摘要: {summary}"
                
                context_parts.append(chunk_text)
            
            context = "\n\n".join(context_parts)
            print(f"📝 构建上下文: context_length={len(context)}, chunks_count={len(context_parts)}")
            
            # 调用 LLM 生成答案
            print(f"🚀 调用 generate_answer...")
            answer_result = await self.tools.generate_answer(
                query=query,
                context=context,
                document_id=document_id
            )
            answer = answer_result["answer"]
            confidence = answer_result.get("confidence")
            
            print(f"✅ 答案生成完成: answer_length={len(answer)}, confidence={confidence}")
            print(f"{'='*60}\n")
            
            # 构建相关文档片段列表
            relevant_chunks = []
            for r in results:
                # 确保 score 在 0-1 范围内
                raw_score = r.get("score", 0.0)
                
                # 调试日志
                if raw_score < 0.0 or raw_score > 1.0:
                    logger.warning(f"⚠️ 检测到异常的 score 值: {raw_score}, 进行修正")
                
                # 如果 score 不在有效范围内，进行修正
                if raw_score < 0.0:
                    # 负数或异常值，使用默认值
                    score = 0.0
                    logger.warning(f"Score 为负数 {raw_score}，已修正为 0.0")
                elif raw_score > 1.0:
                    # 超过 1.0，限制为 1.0
                    score = 1.0
                    logger.warning(f"Score 超过 1.0 ({raw_score})，已修正为 1.0")
                else:
                    score = raw_score
                
                # 从结果中提取信息（document_tools 已经转换过格式）
                chunk_id = r.get("chunk_id", 0)
                chunk_index = r.get("chunk_index", 0)
                content = r.get("content", "")
                summary = r.get("summary")
                
                # 确保 score 是 float 类型且在有效范围内
                final_score = max(0.0, min(1.0, float(score)))
                
                try:
                    relevant_chunks.append(
                        RelevantChunk(
                            chunk_id=int(chunk_id) if chunk_id is not None else 0,
                            chunk_index=int(chunk_index) if chunk_index is not None else 0,
                            content=str(content) if content else "",
                            summary=str(summary) if summary else None,
                            score=final_score
                        )
                    )
                except Exception as e:
                    logger.error(f"构建 RelevantChunk 失败: chunk_id={chunk_id}, chunk_index={chunk_index}, score={final_score}, error={str(e)}")
                    logger.error(f"原始数据: {r}")
                    raise
            
            logger.info(f"文档问答完成: answer_length={len(answer)}, chunks_count={len(relevant_chunks)}")
            
            return DocumentQueryResponseData(
                answer=answer,
                relevant_chunks=relevant_chunks,
                confidence=confidence
            )
            
        except Exception as e:
            logger.error(f"文档问答失败: {str(e)}", exc_info=True)
            raise

