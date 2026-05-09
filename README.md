# Ocelot Microservices Architecture

This repository demonstrates a complete **microservices architecture** using **Ocelot API Gateway** with .NET Core. The system consists of:

- **Product Service** - Manages product CRUD operations
- **Order Service** - Manages order CRUD operations  
- **Ocelot API Gateway** - Routes traffic between services and enforces rate limiting

All services are containerized with **Docker** and orchestrated with **Kubernetes**.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Client                               │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
                    ┌────────────────────┐
                    │   API Gateway      │
                    │   (Ocelot)         │
                    │   Port: 5000/80    │
                    └────────┬───────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         ▼                   ▼                   ▼
    ┌──────────┐         ┌──────────┐
    │ Product  │         │  Order   │
    │ Service  │         │ Service  │
    │ Port:5001│         │ Port:5002│
    └──────────┘         └──────────┘
```

## Quick Start

### Prerequisites
- Docker & Docker Compose
- Kubernetes cluster (for K8s deployment)
- kubectl CLI tool

### Docker Compose (Local Development)

```bash
# Build and start all services
docker-compose up --build

# Verify services are running
docker ps

# Stop services
docker-compose down
```

**Access endpoints:**
- API Gateway: `http://localhost:5000`
- Product Service: `http://localhost:5001`
- Order Service: `http://localhost:5002`

### Kubernetes Deployment

#### Step 1: Create namespace and ConfigMap
```bash
kubectl apply -f kubernetes/01-namespace.yaml
kubectl apply -f kubernetes/02-ocelot-configmap.yaml
```

#### Step 2: Deploy microservices
```bash
kubectl apply -f kubernetes/03-product-service.yaml
kubectl apply -f kubernetes/04-order-service.yaml
kubectl apply -f kubernetes/05-api-gateway.yaml
```

#### Step 3: Verify deployment
```bash
# Check pods
kubectl get pods -n ocelot-microservices

# Check services
kubectl get svc -n ocelot-microservices

# Check deployment status
kubectl describe deployment api-gateway -n ocelot-microservices
```

#### Step 4: Access the API Gateway
```bash
# Get LoadBalancer external IP
kubectl get svc api-gateway -n ocelot-microservices

# Use the EXTERNAL-IP to access services
curl http://<EXTERNAL-IP>/api/product/health
```

## API Endpoints

### Product Service

```bash
# Get all products
curl -X GET http://localhost:5000/api/product

# Get product by ID
curl -X GET http://localhost:5000/api/product/1

# Create product
curl -X POST http://localhost:5000/api/product \
  -H "Content-Type: application/json" \
  -d '{"name":"Laptop","price":999.99}'

# Update product
curl -X PUT http://localhost:5000/api/product/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Laptop","price":1099.99}'

# Delete product
curl -X DELETE http://localhost:5000/api/product/1

# Health check
curl http://localhost:5000/api/product/health
```

### Order Service

```bash
# Get all orders
curl -X GET http://localhost:5000/api/order

# Get order by ID
curl -X GET http://localhost:5000/api/order/1

# Create order
curl -X POST http://localhost:5000/api/order \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":5,"totalPrice":4999.95}'

# Update order
curl -X PUT http://localhost:5000/api/order/1 \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":10,"totalPrice":9999.90}'

# Delete order
curl -X DELETE http://localhost:5000/api/order/1

# Health check
curl http://localhost:5000/api/order/health
```

## Kubernetes Scaling

```bash
# Scale Product Service to 5 replicas
kubectl scale deployment/product-service --replicas=5 -n ocelot-microservices

# Scale Order Service to 5 replicas
kubectl scale deployment/order-service --replicas=5 -n ocelot-microservices

# Scale API Gateway to 3 replicas
kubectl scale deployment/api-gateway --replicas=3 -n ocelot-microservices

# View autoscaling status
kubectl get hpa -n ocelot-microservices
```

## Docker Image Building

To build and push Docker images to your registry:

```bash
# Build images
docker build -t <your-registry>/product-service:latest ./ProductService
docker build -t <your-registry>/order-service:latest ./OrderService
docker build -t <your-registry>/api-gateway:latest ./ApiGateway

# Push to registry
docker push <your-registry>/product-service:latest
docker push <your-registry>/order-service:latest
docker push <your-registry>/api-gateway:latest

# Update Kubernetes manifests with your registry
# Change image: rohitk99/* to image: <your-registry>/*
```

## Key Features

✅ **Ocelot API Gateway** - Centralized routing and rate limiting  
✅ **Microservices** - Independent, scalable services  
✅ **Health Checks** - Liveness and readiness probes  
✅ **Rate Limiting** - 100 requests per minute per service  
✅ **Docker Support** - Multi-stage builds for optimization  
✅ **Kubernetes Ready** - Production-grade manifests  
✅ **Resource Management** - CPU and memory limits defined  
✅ **Service Discovery** - DNS-based service discovery in K8s  

## Troubleshooting

### Pods not starting
```bash
# Check pod logs
kubectl logs <pod-name> -n ocelot-microservices

# Describe pod for events
kubectl describe pod <pod-name> -n ocelot-microservices
```

### Service not accessible
```bash
# Check if service is running
kubectl get svc -n ocelot-microservices

# Port forward to test locally
kubectl port-forward svc/api-gateway 8080:80 -n ocelot-microservices
# Access at http://localhost:8080
```

### Rate limiting issues
Check the Ocelot configuration in `02-ocelot-configmap.yaml` and adjust limits as needed.

## Future Enhancements

- [ ] Add database persistence (SQL Server/PostgreSQL)
- [ ] Implement authentication (JWT/OAuth2)
- [ ] Add distributed caching (Redis)
- [ ] Implement message queues (RabbitMQ/Kafka)
- [ ] Add monitoring and logging (ELK Stack/Prometheus)
- [ ] Implement CI/CD pipeline
- [ ] Add API versioning
- [ ] Implement circuit breaker pattern

## License

MIT License - feel free to use this for your projects!
