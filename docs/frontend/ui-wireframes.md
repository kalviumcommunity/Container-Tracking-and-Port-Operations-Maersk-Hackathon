# PortTrack Container Operations Design Documentation

## 🔗 Figma File
The complete, interactive design prototype and source files are available in Figma:  
[Insert Figma File Link Here](https://www.figma.com/design/DQM6qMcg28thJCOlh0ZgiN/Maersk-Hackathon?node-id=0-1&t=wQ3KksKLuzFnHN6A-1)

---

## 1. Overview
This documentation provides an overview of the design assets for the **PortTrack Container Operations** application, a system for real-time monitoring and management of port and container operations.

The assets are categorized into:
- **Low-Fidelity Wireframes (Low-Fi)** – Initial structural blueprint.  
- **High-Fidelity Mockups (High-Fi)** – Final polished visual design.  

---

## 2. Low-Fidelity Wireframes (Low-Fi)
The low-fidelity wireframes represent the initial structural and functional design of the application. They focus on layout, navigation, and core functionality, deliberately omitting visual details like color, typography, and branding.

### Purpose
- **Structure & Flow**: Define the information architecture and screen flow.  
- **Functionality**: Outline the core elements, actions, and data displayed on each page.  
- **Early Feedback**: Facilitate quick iteration and feedback on UX without visual distractions.  

### Included Screens & Key Elements
- **Port Operations Dashboard**  
  - Performance Metric Cards  
  - Recent Container Activity  
  - Live Port Status panel  

- **Container Management**  
  - Search bar  
  - Filter/Add Container actions  
  - Card view for individual containers with View Details and Update Status actions  

- **Port Operations Management**  
  - Metric Cards  
  - Berth Management grid of berths  
  - Active Operations list with progress indicators  

- **Event Streaming Dashboard**  
  - Metric Cards  
  - Live Event Feed (list of events)  
  - Stream Controls with filters for event type and priority  

---

## 3. High-Fidelity Wireframes (High-Fi)
The high-fidelity mockups apply **PortTrack branding, color palette, iconography, and final layouts** determined during wireframing.

### Purpose
- **Visual Design**: Present final look-and-feel (colors, fonts, imagery).  
- **Clarity & Realism**: Show realistic data, statuses, and UI states.  
- **Developer Handoff**: Serve as the definitive reference for developers.  

### Included Screens & Key Visual Enhancements
- **Port Operations Dashboard**  
  - Branded header with PortTrack logo  
  - Clear operational status indicators  
  - Detailed Berth Utilization and Container Capacity bars  

- **Container Management**  
  - Specific container details (Refrigerated, Dry, Liquid)  
  - Color-coded status tags (At Port, In Transit)  
  - Real-time data (Temperature, Location)  

- **Port Operations Management**  
  - Metric Cards with specific numbers (Total Berths: 15, Occupied: 5)  
  - Color-coded berth status (Occupied, Free, Maintenance)  
  - Active Operations with progress bars and priority tags (High, Medium)  

- **Event Streaming Dashboard**  
  - Real-time Kafka Stream connection status  
  - Event acknowledgment with checkmarks and colors  
  - Tags for New/In Progress events  
  - Filtering by event type and priority  

---

## 4. Design Decisions & Style
The transition from low-fi to high-fi reflects key design improvements:

- **Color Palette**: Professional scheme with **blue** for actions/info, **green/yellow/red** for statuses & priorities.  
- **Data Density**: High-fi components include richer details (e.g., Berth cards now show Ship & Capacity).  
- **Usability Enhancements**: Clear iconography (anchors, containers) and bold typography improve readability & quick scanning.  

---
