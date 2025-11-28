/**
 * Markdown 解析 Composable
 * 注意：如果项目使用了 @nuxtjs/mdc，它会自动提供 parseMarkdown
 * 这个 composable 提供基础的 markdown-it 解析功能
 */
import MarkdownIt from 'markdown-it'

let md: MarkdownIt | null = null

export const useMarkdown = () => {
  if (!md) {
    md = new MarkdownIt({
      html: true,
      linkify: true,
      typographer: true
    })
  }

  return {
    parse: (markdown: string) => {
      return md!.render(markdown)
    }
  }
}

// 重命名以避免与 @nuxtjs/mdc 的 parseMarkdown 冲突
export const parseMarkdownToHtml = async (markdown: string) => {
  const { parse } = useMarkdown()
  const html = parse(markdown)
  
  return {
    body: {
      children: [
        {
          tag: 'div',
          props: {},
          children: html
        }
      ]
    }
  }
}

