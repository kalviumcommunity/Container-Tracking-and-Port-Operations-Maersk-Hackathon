#!/bin/bash

# Container Tracking API - Authentication Testing Script
# This script helps you quickly test JWT authentication and API endpoints

BASE_URL="${1:-http://localhost:5221}"
USERNAME="${2:-admin}"
PASSWORD="${3:-Admin123!}"

echo "🔐 Container Tracking API - Authentication Testing"
echo "Base URL: $BASE_URL"
echo "Username: $USERNAME"
echo ""

# Function to make API requests
api_request() {
    local url="$1"
    local method="${2:-GET}"
    local data="$3"
    local description="$4"
    local headers="$5"
    
    echo "📡 $description"
    echo "   $method $url"
    
    if [ -n "$data" ]; then
        if [ -n "$headers" ]; then
            response=$(curl -s -X "$method" "$url" -H "Content-Type: application/json" -H "$headers" -d "$data")
        else
            response=$(curl -s -X "$method" "$url" -H "Content-Type: application/json" -d "$data")
        fi
    else
        if [ -n "$headers" ]; then
            response=$(curl -s -X "$method" "$url" -H "$headers")
        else
            response=$(curl -s -X "$method" "$url")
        fi
    fi
    
    # Check if response contains error
    if echo "$response" | grep -q '"error":\|"message":\|"title":'; then
        echo "   ❌ Error: $response"
        return 1
    else
        echo "   ✅ Success"
        echo "$response"
        return 0
    fi
}

# Step 1: Login and get JWT token
echo "Step 1: Authenticating..."
login_data="{\"username\":\"$USERNAME\",\"password\":\"$PASSWORD\"}"

auth_response=$(api_request "$BASE_URL/api/auth/login" "POST" "$login_data" "Login")

if [ $? -ne 0 ]; then
    echo "❌ Authentication failed. Make sure the API is running and credentials are correct."
    exit 1
fi

# Extract token from response
token=$(echo "$auth_response" | grep -o '"token":"[^"]*' | cut -d'"' -f4)
username=$(echo "$auth_response" | grep -o '"username":"[^"]*' | cut -d'"' -f4)
email=$(echo "$auth_response" | grep -o '"email":"[^"]*' | cut -d'"' -f4)

if [ -z "$token" ]; then
    echo "❌ Failed to extract token from response"
    exit 1
fi

echo ""
echo "🎉 Authentication Successful!"
echo "Username: $username"
echo "Email: $email"
echo "Token: ${token:0:50}..."
echo ""

# Create authorization header
auth_header="Authorization: Bearer $token"

# Step 2: Test user profile endpoint
echo "Step 2: Testing authenticated endpoints..."
profile_response=$(api_request "$BASE_URL/api/auth/profile" "GET" "" "Get user profile" "$auth_header")

# Step 3: Test containers endpoint
containers_response=$(api_request "$BASE_URL/api/containers" "GET" "" "Get containers" "$auth_header")

# Count containers
container_count=$(echo "$containers_response" | grep -o '"id":' | wc -l)
echo "   Found $container_count containers"

# Step 4: Test ports endpoint
ports_response=$(api_request "$BASE_URL/api/ports" "GET" "" "Get ports" "$auth_header")

# Count ports
port_count=$(echo "$ports_response" | grep -o '"id":' | wc -l)
echo "   Found $port_count ports"

# Step 5: Test ships endpoint
ships_response=$(api_request "$BASE_URL/api/ships" "GET" "" "Get ships" "$auth_header")

# Count ships
ship_count=$(echo "$ships_response" | grep -o '"id":' | wc -l)
echo "   Found $ship_count ships"

# Step 6: Test admin-only endpoint (if admin user)
if echo "$auth_response" | grep -q '"Admin"'; then
    echo ""
    echo "Step 3: Testing admin endpoints..."
    users_response=$(api_request "$BASE_URL/api/auth/users" "GET" "" "Get all users (Admin only)" "$auth_header")
    
    user_count=$(echo "$users_response" | grep -o '"id":' | wc -l)
    echo "   Found $user_count users in system"
fi

echo ""
echo "🎯 Testing Complete!"
echo ""
echo "💡 Tips:"
echo "   - Save the JWT token for use in other tools:"
echo "     export JWT_TOKEN='$token'"
echo "   - Use the token in curl commands:"
echo "     curl -H 'Authorization: Bearer \$JWT_TOKEN' $BASE_URL/api/containers"
echo "   - Import the Postman collection for easier testing:"
echo "     docs/Container-Tracking-API-Auth.postman_collection.json"
echo ""

# Export token to environment variable
export JWT_TOKEN="$token"
echo "🔑 JWT token exported to JWT_TOKEN environment variable"

# Save token to file for persistence
echo "$token" > .jwt_token
echo "🔑 JWT token saved to .jwt_token file"

echo ""
echo "🚀 Quick Test Commands:"
echo "   curl -H \"Authorization: Bearer \$JWT_TOKEN\" $BASE_URL/api/containers"
echo "   curl -H \"Authorization: Bearer \$JWT_TOKEN\" $BASE_URL/api/ports"
echo "   curl -H \"Authorization: Bearer \$JWT_TOKEN\" $BASE_URL/api/ships"
echo ""