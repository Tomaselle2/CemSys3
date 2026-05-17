import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [react()],
    build: {
        outDir: '../wwwroot/reactflow',
        emptyOutDir: true,
        rollupOptions: {
            output: {
                entryFileNames: 'app.js',
                assetFileNames: 'app.css'
            }
        }
    }
})
