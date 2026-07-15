# === Step 1: Build project ===
FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci --silent

COPY . .T
RUN npm run build -- --configuration=production

# === Step 2: Nginx ===
FROM nginx:1.31.2-alpine AS production

COPY --from=build /app/dist/food-race-fe/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
