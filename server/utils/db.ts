import mysql from 'mysql2/promise'

// Create a connection pool
const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
})

// Initialize database table
export const initDB = async () => {
    try {
        const connection = await pool.getConnection()
        await connection.query(`
      CREATE TABLE IF NOT EXISTS visit_logs (
        id VARCHAR(36) PRIMARY KEY,
        visitor_id VARCHAR(36) NOT NULL,
        ip VARCHAR(45),
        user_agent TEXT,
        path VARCHAR(255),
        timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
      )
    `)
        connection.release()
    } catch (error) {
        console.error('Database initialization failed:', error)
    }
}

export const db = pool
