"""
文档知识管家 Agent 服务
处理文档解析、分段、摘要、知识结构构建、向量化、问答等核心功能
"""

from typing import List, Dict, Any, Optional
from app.core.logging import logger
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
                        "chunk_index": i,
                        "content": chunk[:200],  # 只存储前 200 字符作为元数据
                        "summary": chunk_summary
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
            logger.info(f"开始文档问答: document_id={document_id}, query={query[:50]}...")
            
            # 1. 将问题向量化
            embed_result = await self.tools.embed_text(
                text=query,
                model_name="text-embedding-ada-002"
            )
            query_vector = embed_result["vector"]
            
            # 2. 在向量数据库中检索
            search_result = await self.tools.search_vectors(
                query_vector=query_vector,
                top_k=top_k,
                document_id=document_id  # 限定在当前文档
            )
            results = search_result["results"]
            
            logger.info(f"向量检索完成: results_count={len(results)}")
            
            # 3. 使用 LLM 生成答案
            # 构建上下文（相关文档片段）
            context = "\n\n".join([
                f"[片段 {r['chunk_index']}]: {r['content']}"
                for r in results
            ])
            
            # 调用 LLM 生成答案
            answer_result = await self.tools.generate_answer(
                query=query,
                context=context,
                document_id=document_id
            )
            answer = answer_result["answer"]
            confidence = answer_result.get("confidence")
            
            # 构建相关文档片段列表
            relevant_chunks = [
                RelevantChunk(
                    chunk_id=r.get("chunk_id", 0),
                    chunk_index=r.get("chunk_index", 0),
                    content=r.get("content", ""),
                    summary=r.get("summary"),
                    score=r.get("score", 0.0)
                )
                for r in results
            ]
            
            logger.info(f"文档问答完成: answer_length={len(answer)}, chunks_count={len(relevant_chunks)}")
            
            return DocumentQueryResponseData(
                answer=answer,
                relevant_chunks=relevant_chunks,
                confidence=confidence
            )
            
        except Exception as e:
            logger.error(f"文档问答失败: {str(e)}", exc_info=True)
            raise

