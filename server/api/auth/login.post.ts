export default defineEventHandler(async (event) => {
    const body = await readBody(event)
    const { password } = body

    // In a real app, use a proper env variable. 
    // For this personal site, we'll hardcode a default or check env.
    // The user asked for "only I know", so a simple password check is sufficient.
    const adminPassword = process.env.ADMIN_PASSWORD || 'admin123'

    console.log('Login attempt. Input:', password, 'Expected:', adminPassword)

    if (password === adminPassword) {
        // Set a cookie that expires in 1 day
        setCookie(event, 'admin_auth', 'true', {
            maxAge: 60 * 60 * 24,
            httpOnly: false, // Allow client-side access for middleware checks
            path: '/'
        })
        return {
            success: true,
            token: 'admin_token_placeholder', // Dummy token for frontend useApi
            username: 'admin',
            role: 'admin'
        }
    }

    throw createError({
        statusCode: 401,
        statusMessage: 'Unauthorized'
    })
})
