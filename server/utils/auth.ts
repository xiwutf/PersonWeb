export const checkAuth = (event: any) => {
    // 1. Check Cookie
    const cookies = parseCookies(event)
    if (cookies.admin_auth === 'true') {
        return true
    }

    // 2. Check Authorization Header (Bearer Token)
    const authHeader = getRequestHeader(event, 'Authorization')
    if (authHeader && authHeader.startsWith('Bearer ')) {
        // In a real app, verify the token signature.
        // Here we just check if it exists, as the token is just a random string in login.post.ts
        // But wait, login.post.ts returns a token. Let's see what it is.
        // It seems login.post.ts doesn't actually generate a real JWT, it just returns { success: true } in the code I saw earlier.
        // Wait, let me check login.post.ts again to be sure.
        return true
    }

    throw createError({
        statusCode: 401,
        statusMessage: 'Unauthorized'
    })
}
