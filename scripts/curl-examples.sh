# Quick cURL Examples for Container Tracking API Authentication

# Base URL
BASE_URL="http://localhost:5221"

echo "🔐 Container Tracking API - cURL Testing Examples"
echo "=================================================="
echo ""

# 1. Login and get JWT token
echo "1. Login (Get JWT Token):"
echo "curl -X POST \"$BASE_URL/api/auth/login\" \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{\"username\":\"admin\",\"password\":\"Admin123!\"}'"
echo ""

# 2. Save token to variable
echo "2. Save token to variable (replace <TOKEN> with actual token):"
echo "export JWT_TOKEN=\"<YOUR_TOKEN_HERE>\""
echo ""

# 3. Test authenticated endpoints
echo "3. Test Authenticated Endpoints:"
echo ""

echo "Get user profile:"
echo "curl -H \"Authorization: Bearer \$JWT_TOKEN\" \"$BASE_URL/api/auth/profile\""
echo ""

echo "Get all containers:"
echo "curl -H \"Authorization: Bearer \$JWT_TOKEN\" \"$BASE_URL/api/containers\""
echo ""

echo "Get all ports:"
echo "curl -H \"Authorization: Bearer \$JWT_TOKEN\" \"$BASE_URL/api/ports\""
echo ""

echo "Get all ships:"
echo "curl -H \"Authorization: Bearer \$JWT_TOKEN\" \"$BASE_URL/api/ships\""
echo ""

echo "Get all users (Admin only):"
echo "curl -H \"Authorization: Bearer \$JWT_TOKEN\" \"$BASE_URL/api/auth/users\""
echo ""

# 4. Create operations
echo "4. Create Operations:"
echo ""

echo "Create new container:"
echo "curl -X POST \"$BASE_URL/api/containers\" \\"
echo "  -H \"Authorization: Bearer \$JWT_TOKEN\" \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{\"containerNumber\":\"TEST001\",\"size\":\"20ft\",\"type\":\"Standard\",\"status\":\"Available\"}'"
echo ""

echo "Create new user (Admin only):"
echo "curl -X POST \"$BASE_URL/api/auth/register\" \\"
echo "  -H \"Authorization: Bearer \$JWT_TOKEN\" \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{\"username\":\"newuser\",\"email\":\"newuser@example.com\",\"password\":\"NewUser123!\",\"roles\":[\"Operator\"]}'"
echo ""

# 5. Complete example
echo "5. Complete Example (Copy and paste this):"
echo "=========================================="
echo ""
echo "# Login and get token"
echo "TOKEN=\$(curl -s -X POST \"$BASE_URL/api/auth/login\" \\"
echo "  -H \"Content-Type: application/json\" \\"
echo "  -d '{\"username\":\"admin\",\"password\":\"Admin123!\"}' | \\"
echo "  grep -o '\"token\":\"[^\"]*' | cut -d'\"' -f4)"
echo ""
echo "# Test the token"
echo "curl -H \"Authorization: Bearer \$TOKEN\" \"$BASE_URL/api/auth/profile\""
echo ""

echo "💡 Pro Tips:"
echo "- Use jq for better JSON formatting: curl ... | jq"
echo "- Check API documentation at: $BASE_URL/swagger"
echo "- Import Postman collection for easier testing"
echo ""