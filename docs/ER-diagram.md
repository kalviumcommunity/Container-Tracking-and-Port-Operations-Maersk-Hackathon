# 🚢 Container Tracking & Port Operations System

This project manages **container lifecycle tracking, port operations, berth management, and real-time event updates** for port logistics.

---

## 📊 ER Diagram & Workflow

Here’s the ER diagram and workflow of the project:

![ER & Workflow Diagram](https://i.postimg.cc/mZ592xMT/Screenshot-2025-09-27-152440.png)

---

## 🗄 Database Design (PostgreSQL)

The database consists of **five main entities**:

### **1. PORT**
- **Attributes**: `PortId (PK)`, Name, Location, TotalCapacity  
- **Relationships**:  
  - A port has many **ships** (`PortId FK` in Ship).  
  - A port has many **berths** (`PortId FK` in Berth).  

🔑 A **port** is the hub where all activities happen.

---

### **2. CONTAINER**
- **Attributes**: `ContainerId (PK)`, Name, Type, Status, Location  
- **Relationships**:  
  - Connected to **Assignment** (tracks which container is on which ship/berth).  

🔑 A **container** is the basic cargo unit being tracked.

---

### **3. SHIP**
- **Attributes**: `ShipId (PK)`, Name, Status, `PortId (FK)`  
- **Relationships**:  
  - Linked to **Assignment** (ship carries containers).  

🔑 A **ship** brings containers into the port.

---

### **4. BERTH**
- **Attributes**: `BerthId (PK)`, Status, Capacity, `PortId (FK)`  
- **Relationships**:  
  - Linked to **Assignment** (ships and containers dock here).  

🔑 A **berth** is the physical place where ships dock.

---

### **5. ASSIGNMENT**
- **Attributes**: `AssignmentId (PK)`, `ContainerId (FK)`, `ShipId (FK)`, `BerthId (FK)`, Timestamp  
- **Relationships**:  
  - Connects **containers → ships → berths**.  

🔑 This is the **linking entity** that logs container movements.

---

📌 **Database Big Picture:**  
- Ports manage ships and berths.  
- Ships carry containers.  
- Assignments record which container is on which ship at which berth, and when.  

---

## ⚙️ Workflow of the Project

The workflow shows how the system components interact:

### **1. Frontend (Vue.js UI)**
- Interface for port operators and staff.  
- Handles **CRUD** operations like adding ships, containers, or berth assignments.  

### **2. Backend (.NET Core APIs)**
- Contains business logic and API endpoints.  
- Handles CRUD requests, validates capacity, assigns containers.  
- Publishes **real-time events** (e.g., *Ship X docked at Berth Y*).  

### **3. Database (PostgreSQL)**
- Stores structured data (Ports, Ships, Containers, Berths, Assignments).  
- Accessed by backend through SQL queries.  

### **4. Kafka Event Streaming**
- Publishes and consumes **real-time events** (ship arrival, berth availability, container status).  
- Frontend consumes events → provides **live updates without page refresh**.  

---

📌 **Workflow Big Picture:**  
- User action → **Frontend (Vue.js)** → **API (.NET Core)** → **Database (Postgres)**  
- System event → **Backend publishes** → **Kafka** → **Frontend consumes → live update shown**  

---

## 🛠 Tech Stack

- **Backend:** .NET Core  
- **Frontend:** Vue.js  
- **Database:** PostgreSQL  
- **Event Streaming:** Apache Kafka  

---

## 👥 Team

- Snegan Palanisamy  
- Dhruvil Deepak Sheth  

---

