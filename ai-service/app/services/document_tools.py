"""
文档知识管家 Agent 工具函数
提供文档解析、分段、摘要、知识结构、向量化等工具
"""

from typing import Dict, List, Any, Optional
from app.core.logging import logger
from app.core.llm_client import llm_client


class DocumentTools:
    """文档处理工具类"""
    
    async def parse_document(
        self,
        file_path: str,
        file_type: str
    ) -> Dict[str, Any]:
        """
        解析文档工具
        
        解析 PDF/DOCX/TXT 等格式文档，提取文本内容
        
        Args:
            file_path: 文件路径
            file_type: 文件类型 (pdf, docx, txt, md)
            
        Returns:
            dict: {
                "raw_text": str,  # 原始文本
                "metadata": dict   # 文档元数据 (页数、标题等)
            }
        """
        logger.info(f"解析文档: file_path={file_path}, file_type={file_type}")
        
        try:
            # 检查文件是否存在
            import os
            if not os.path.exists(file_path):
                error_msg = f"文件不存在: {file_path}"
                logger.error(error_msg)
                raise FileNotFoundError(error_msg)
            
            # TODO: 实现真实的文档解析逻辑
            # 1. 根据 file_type 选择对应的解析库
            #    - PDF: PyPDF2, pdfplumber
            #    - DOCX: python-docx
            #    - TXT/MD: 直接读取
            # 2. 提取文本和元数据
            
            # 当前为简单实现：读取文本文件
            if file_type in ["txt", "md"]:
                try:
                    with open(file_path, "r", encoding="utf-8") as f:
                        raw_text = f.read()
                except UnicodeDecodeError:
                    # 如果 UTF-8 失败，尝试其他编码
                    logger.warning(f"UTF-8 解码失败，尝试其他编码: {file_path}")
                    with open(file_path, "r", encoding="gbk") as f:
                        raw_text = f.read()
            else:
                # 其他格式暂时返回占位文本
                logger.warning(f"不支持的文件类型 {file_type}，返回占位文本")
                raw_text = f"[文档内容占位符] 文件类型: {file_type}, 路径: {file_path}"
            
            metadata = {
                "file_type": file_type,
                "file_path": file_path,
                "title": os.path.basename(file_path) or "未命名文档"
            }
            
            logger.info(f"文档解析完成: text_length={len(raw_text)}")
            
            return {
                "raw_text": raw_text,
                "metadata": metadata
            }
        except FileNotFoundError:
            # 重新抛出文件不存在错误
            raise
        except Exception as e:
            logger.error(f"解析文档失败: {str(e)}", exc_info=True)
            raise
    
    async def chunk_text(
        self,
        text: str,
        chunk_size: int = 1000,
        chunk_overlap: int = 200
    ) -> Dict[str, Any]:
        """
        文本分段工具
        
        将长文本按照固定大小分段，支持重叠
        
        Args:
            text: 文本内容
            chunk_size: 分段大小（字符数）
            chunk_overlap: 重叠大小（字符数）
            
        Returns:
            dict: {
                "chunks": List[str],  # 分段列表
                "chunk_metadata": List[dict]  # 每个分段的元数据
            }
        """
        logger.info(f"文本分段: text_length={len(text)}, chunk_size={chunk_size}, overlap={chunk_overlap}")
        
        try:
            # 如果文本为空或很短，直接返回单个分段
            if not text or len(text) <= chunk_size:
                return {
                    "chunks": [text] if text else [],
                    "chunk_metadata": [{
                        "index": 0,
                        "start": 0,
                        "end": len(text),
                        "length": len(text)
                    }] if text else []
                }
            
            chunks = []
            chunk_metadata = []
            
            # 确保 chunk_overlap 不会导致无限循环
            # 如果 overlap 太大，限制为 chunk_size 的一半
            if chunk_overlap >= chunk_size:
                chunk_overlap = max(1, chunk_size // 2)
                logger.warning(f"chunk_overlap 过大，已调整为: {chunk_overlap}")
            
            # 简单实现：按固定大小分段
            start = 0
            index = 0
            
            while start < len(text):
                end = min(start + chunk_size, len(text))
                chunk = text[start:end]
                chunks.append(chunk)
                
                chunk_metadata.append({
                    "index": index,
                    "start": start,
                    "end": end,
                    "length": len(chunk)
                })
                
                # 移动到下一个分段（考虑重叠）
                # 确保 start 总是递增，避免无限循环
                next_start = end - chunk_overlap
                if next_start <= start:
                    # 如果下一个 start 没有递增，强制递增至少 1
                    next_start = start + 1
                
                # 如果已经到达文本末尾，退出循环
                if end >= len(text):
                    break
                    
                start = next_start
                index += 1
                
                # 安全保护：如果分段数量过多，停止处理
                if index > 10000:
                    logger.error(f"分段数量过多 ({index})，可能存在无限循环，停止处理")
                    raise ValueError(f"分段数量过多，可能存在逻辑错误")
            
            logger.info(f"文本分段完成: chunks_count={len(chunks)}")
            
            return {
                "chunks": chunks,
                "chunk_metadata": chunk_metadata
            }
        except Exception as e:
            logger.error(f"文本分段失败: {str(e)}", exc_info=True)
            raise
    
    async def generate_summary(
        self,
        text: str,
        summary_type: str = "document"
    ) -> Dict[str, Any]:
        """
        生成摘要工具
        
        使用 LLM 生成文档或分段的摘要
        
        Args:
            text: 文本内容
            summary_type: 摘要类型 ("document" 或 "chunk")
            
        Returns:
            dict: {
                "summary": str,  # 摘要文本
                "key_points": List[str]  # 关键点列表（可选）
            }
        """
        logger.info(f"生成摘要: text_length={len(text)}, summary_type={summary_type}")
        
        try:
            # 构建提示词
            if summary_type == "chunk":
                system_prompt = "你是一个专业的文本摘要助手。请为给定的文本片段生成简洁准确的摘要，控制在100字以内。"
                user_prompt = f"请为以下文本片段生成摘要：\n\n{text}"
            else:
                system_prompt = "你是一个专业的文档摘要助手。请为给定的文档生成全面准确的摘要，控制在300字以内，并提取3-5个关键点。"
                user_prompt = f"请为以下文档生成摘要和关键点：\n\n{text}"
            
            # 调用 LLM
            result = await llm_client.chat(
                prompt=user_prompt,
                system_prompt=system_prompt,
                temperature=0.3,  # 降低温度以获得更准确的摘要
                max_tokens=500 if summary_type == "document" else 200
            )
            
            summary = result["reply"].strip()
            
            # 尝试提取关键点（从摘要中提取或单独生成）
            key_points = []
            if summary_type == "document":
                # 尝试从回复中提取关键点（如果 LLM 返回了列表格式）
                lines = summary.split('\n')
                for line in lines:
                    line = line.strip()
                    # 检查是否是关键点格式（以数字、-、• 开头）
                    if line and (line[0].isdigit() or line.startswith('-') or line.startswith('•')):
                        key_point = line.lstrip('0123456789.-• ').strip()
                        if key_point:
                            key_points.append(key_point)
            
            # 如果没有提取到关键点，且是文档摘要，尝试单独生成
            if summary_type == "document" and not key_points:
                try:
                    key_points_prompt = f"请从以下文档中提取3-5个关键点，每行一个：\n\n{text[:2000]}"  # 限制长度
                    key_points_result = await llm_client.chat(
                        prompt=key_points_prompt,
                        system_prompt="你是一个专业的信息提取助手。请从文档中提取关键点，每行一个。",
                        temperature=0.3,
                        max_tokens=200
                    )
                    key_points = [
                        line.strip().lstrip('0123456789.-• ')
                        for line in key_points_result["reply"].split('\n')
                        if line.strip() and not line.strip().startswith('#')
                    ][:5]  # 最多5个
                except Exception as e:
                    logger.warning(f"提取关键点失败: {str(e)}")
            
            logger.info(f"摘要生成完成: summary_length={len(summary)}, key_points_count={len(key_points)}")
            
            return {
                "summary": summary,
                "key_points": key_points
            }
        except Exception as e:
            logger.error(f"生成摘要失败: {str(e)}", exc_info=True)
            # 失败时返回简单截断作为后备
            max_length = 200 if summary_type == "chunk" else 500
            summary = text[:max_length] + "..." if len(text) > max_length else text
            return {
                "summary": summary,
                "key_points": []
            }
    
    async def build_knowledge_structure(
        self,
        chunks: List[str],
        document_title: str
    ) -> str:
        """
        构建知识结构工具
        
        分析文档内容，构建知识图谱或层次结构
        
        Args:
            chunks: 分段列表
            document_title: 文档标题
            
        Returns:
            str: 知识结构 (JSON 字符串)
        """
        logger.info(f"构建知识结构: document_title={document_title}, chunks_count={len(chunks)}")
        
        try:
            # TODO: 实现真实的知识结构构建逻辑
            # 1. 分析文档内容，提取主题、章节、实体等
            # 2. 构建层次结构或知识图谱
            # 3. 返回 JSON 格式的知识结构
            
            # 当前为简单实现：返回基本结构
            knowledge_structure = {
                "title": document_title,
                "sections": [
                    {
                        "title": f"章节 {i+1}",
                        "chunk_indices": [i]
                    }
                    for i in range(len(chunks))
                ],
                "topics": [],
                "relationships": []
            }
            
            import json
            structure_json = json.dumps(knowledge_structure, ensure_ascii=False)
            
            logger.info("知识结构构建完成")
            
            return structure_json
        except Exception as e:
            logger.error(f"构建知识结构失败: {str(e)}", exc_info=True)
            raise
    
    async def embed_text(
        self,
        text: str,
        model_name: str = "text-embedding-ada-002"
    ) -> Dict[str, Any]:
        """
        向量化工具
        
        将文本转换为向量表示
        
        Args:
            text: 文本内容
            model_name: 模型名称
            
        Returns:
            dict: {
                "vector": List[float],  # 向量数据
                "dimension": int  # 向量维度
            }
        """
        logger.info(f"向量化文本: text_length={len(text)}, model={model_name}")
        
        try:
            # TODO: 实现真实的向量化逻辑
            # 1. 调用 Embedding API（如 OpenAI Embeddings）
            # 2. 返回向量数据
            
            # 当前为简单实现：返回随机向量（仅用于测试）
            dimension = 1536  # OpenAI ada-002 的维度
            import random
            vector = [random.random() for _ in range(dimension)]
            
            logger.info(f"向量化完成: dimension={dimension}")
            
            return {
                "vector": vector,
                "dimension": dimension
            }
        except Exception as e:
            logger.error(f"向量化失败: {str(e)}", exc_info=True)
            raise
    
    async def store_vectors(
        self,
        vectors: List[List[float]],
        metadata: List[Dict[str, Any]]
    ) -> Dict[str, Any]:
        """
        向量存储工具
        
        将向量存储到向量数据库
        
        Args:
            vectors: 向量列表
            metadata: 元数据列表
            
        Returns:
            dict: {
                "success": bool,
                "stored_count": int,
                "vector_ids": List[str]  # 存储后的向量 ID 列表
            }
        """
        logger.info(f"存储向量: vectors_count={len(vectors)}")
        
        try:
            # TODO: 实现真实的向量存储逻辑
            # 1. 连接向量数据库（如 Chroma、Milvus、Qdrant）
            # 2. 存储向量和元数据
            # 3. 返回存储的向量 ID
            
            # 当前为简单实现：只记录日志
            vector_ids = [f"vector_{i}_{hash(str(v))}" for i, v in enumerate(vectors)]
            
            logger.info(f"向量存储完成: stored_count={len(vector_ids)}")
            
            return {
                "success": True,
                "stored_count": len(vector_ids),
                "vector_ids": vector_ids
            }
        except Exception as e:
            logger.error(f"向量存储失败: {str(e)}", exc_info=True)
            raise
    
    async def search_vectors(
        self,
        query_vector: List[float],
        top_k: int = 5,
        document_id: Optional[str] = None
    ) -> Dict[str, Any]:
        """
        向量检索工具
        
        在向量数据库中检索相似文档片段
        
        Args:
            query_vector: 查询向量
            top_k: 返回数量
            document_id: 文档 ID（可选，限定文档）
            
        Returns:
            dict: {
                "results": List[dict]  # 检索结果列表
                    {
                        "chunk_id": int,
                        "document_id": str,
                        "score": float,
                        "content": str,
                        "summary": str,
                        "chunk_index": int
                    }
            }
        """
        logger.info(f"向量检索: top_k={top_k}, document_id={document_id}")
        
        try:
            # TODO: 实现真实的向量检索逻辑
            # 1. 在向量数据库中检索相似向量
            # 2. 根据 document_id 过滤（如果提供）
            # 3. 返回相似度最高的 top_k 个结果
            
            # 当前为简单实现：返回模拟数据
            results = [
                {
                    "chunk_id": i,
                    "document_id": document_id or "unknown",
                    "score": 0.9 - i * 0.1,
                    "content": f"这是文档片段 {i} 的内容...",
                    "summary": f"片段 {i} 的摘要",
                    "chunk_index": i
                }
                for i in range(top_k)
            ]
            
            logger.info(f"向量检索完成: results_count={len(results)}")
            
            return {
                "results": results
            }
        except Exception as e:
            logger.error(f"向量检索失败: {str(e)}", exc_info=True)
            raise
    
    async def generate_answer(
        self,
        query: str,
        context: str,
        document_id: str
    ) -> Dict[str, Any]:
        """
        问答生成工具
        
        基于检索到的文档片段，使用 LLM 生成答案
        
        Args:
            query: 用户问题
            context: 上下文文档片段
            document_id: 文档 ID
            
        Returns:
            dict: {
                "answer": str,  # 答案文本
                "confidence": float  # 置信度 (0-1)
            }
        """
        logger.info(f"生成答案: query={query[:50]}..., context_length={len(context)}")
        
        try:
            # 构建提示词
            system_prompt = """你是一个专业的文档问答助手。请根据提供的文档片段内容，准确回答用户的问题。

要求：
1. 答案必须基于提供的文档内容，不要编造信息
2. 如果文档中没有相关信息，请明确说明"根据提供的文档内容，无法找到相关信息"
3. 答案要简洁明了，重点突出
4. 如果涉及多个片段的内容，请综合整理"""
            
            user_prompt = f"""用户问题：{query}

相关文档片段：
{context}

请根据以上文档片段回答用户的问题。"""
            
            # 调用 LLM
            result = await llm_client.chat(
                prompt=user_prompt,
                system_prompt=system_prompt,
                temperature=0.3,  # 降低温度以获得更准确的答案
                max_tokens=1000
            )
            
            answer = result["reply"].strip()
            
            # 计算置信度（简单启发式方法）
            # 如果答案中包含"无法找到"、"没有相关信息"等，置信度较低
            low_confidence_keywords = ["无法找到", "没有相关信息", "文档中没有", "无法确定"]
            confidence = 0.3 if any(keyword in answer for keyword in low_confidence_keywords) else 0.85
            
            # 如果答案太短，可能置信度较低
            if len(answer) < 20:
                confidence = max(0.3, confidence - 0.2)
            
            logger.info(f"答案生成完成: answer_length={len(answer)}, confidence={confidence}")
            
            return {
                "answer": answer,
                "confidence": confidence
            }
        except Exception as e:
            logger.error(f"生成答案失败: {str(e)}", exc_info=True)
            # 失败时返回默认答案
            return {
                "answer": f"抱歉，生成答案时出现错误：{str(e)}。请稍后重试。",
                "confidence": 0.1
            }

