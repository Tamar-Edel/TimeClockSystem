import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Always use port 5173 in development.
    // strictPort: true means Vite will fail with a clear error instead of
    // silently switching to 5174 or 5175 when the port is already in use.
    // This keeps the port in sync with the backend CORS configuration.
    port: 5173,
    strictPort: true,
  },
})
