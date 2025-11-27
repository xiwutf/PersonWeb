export const parseFrontmatter = (content: string) => {
    const match = content.match(/^---\r?\n([\s\S]*?)\r?\n---\r?\n([\s\S]*)$/)
    if (match) {
        const frontmatterRaw = match[1]
        const body = match[2]
        const data: any = {}
        frontmatterRaw.split(/\r?\n/).forEach(line => {
            const parts = line.split(':')
            if (parts.length >= 2) {
                const key = parts[0].trim()
                let value = parts.slice(1).join(':').trim()

                // Remove quotes if present
                if ((value.startsWith('"') && value.endsWith('"')) || (value.startsWith("'") && value.endsWith("'"))) {
                    value = value.slice(1, -1)
                }

                // Handle arrays like [a, b]
                if (value.startsWith('[') && value.endsWith(']')) {
                    data[key] = value.slice(1, -1).split(',').map(v => v.trim().replace(/^"|"$/g, '').replace(/^'|'$/g, ''))
                } else {
                    data[key] = value
                }
            }
        })
        return { data, content: body }
    }
    return { data: {}, content }
}

export const stringifyFrontmatter = (content: string, data: any) => {
    let frontmatter = '---\n'
    for (const key in data) {
        if (data[key] !== undefined && data[key] !== null) {
            if (Array.isArray(data[key])) {
                frontmatter += `${key}: [${data[key].map((v: string) => `"${v}"`).join(', ')}]\n`
            } else {
                frontmatter += `${key}: ${data[key]}\n`
            }
        }
    }
    frontmatter += '---\n'
    return frontmatter + content
}
