/**
 * Markdown 解析 Composable
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

export const parseMarkdown = async (markdown: string) => {
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

