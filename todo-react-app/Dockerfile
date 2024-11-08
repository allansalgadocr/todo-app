# Stage 1: Build the React app
FROM node:18-alpine AS build-env

# Set working directory
WORKDIR /app

# Install pnpm globally
RUN npm install -g pnpm

# Copy package.json and pnpm-lock.yaml
COPY package.json pnpm-lock.yaml ./

# Install dependencies
RUN pnpm install --frozen-lockfile

# Copy the rest of the application code
COPY . .

# Set environment variables for Vite
ARG VITE_API_BASE_URL
ARG VITE_API_KEY
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_API_KEY=$VITE_API_KEY

# Set Node environment variable
ARG NODE_ENV=production
ENV NODE_ENV=$NODE_ENV

# Build the React app with Vite
RUN pnpm run build

# Stage 2: Serve with Nginx
FROM nginx:stable-alpine

# Copy the build output to Nginx's default HTML directory
COPY --from=build-env /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx server
CMD ["nginx", "-g", "daemon off;"]