# Stage 1: Build React for Docker (mode docker → .env.docker, API via Nginx /api)
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build:docker

# Stage 2: Nginx serves UI + proxies /api to gateway-service
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
