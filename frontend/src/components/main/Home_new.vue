<template>
  <div class="home-container">
    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-background">
        <div class="hero-overlay"></div>
        <img 
          src="https://images.unsplash.com/photo-1541976590-713941681591?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3" 
          alt="Port Operations" 
          class="hero-image"
        />
      </div>
      
      <div class="hero-content">
        <div class="container">
          <div class="hero-text">
            <div class="brand-badge">
              <Ship :size="20" />
              <span>PortTrack Operations</span>
            </div>
            <h1 class="hero-title">
              Next-Generation
              <span class="title-highlight">Port Management</span>
              System
            </h1>
            <p class="hero-subtitle">
              Streamline your maritime operations with advanced container tracking, 
              real-time vessel monitoring, and intelligent port resource management. 
              Trusted by ports worldwide for operational excellence.
            </p>
            <div class="hero-actions">
              <button @click="navigateToDashboard" class="btn-primary">
                <span>Get Started</span>
                <ArrowRight :size="18" />
              </button>
              <button class="btn-secondary">
                <Play :size="18" />
                <span>Watch Demo</span>
              </button>
            </div>
          </div>
          
          <!-- Performance Metrics -->
          <div class="metrics-grid">
            <div v-for="(metric, index) in performanceMetrics" :key="index" class="metric-card">
              <div class="metric-icon" :style="{ backgroundColor: metric.color }">
                <component :is="metric.icon" :size="24" />
              </div>
              <div class="metric-content">
                <div class="metric-value">{{ metric.value }}</div>
                <div class="metric-label">{{ metric.label }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Services Section -->
    <section class="services-section">
      <div class="container">
        <div class="section-header">
          <div class="section-badge">
            <Zap :size="16" />
            <span>Our Services</span>
          </div>
          <h2 class="section-title">Comprehensive Port Solutions</h2>
          <p class="section-subtitle">
            Advanced technology solutions designed to optimize every aspect of your port operations
          </p>
        </div>

        <div class="services-grid">
          <!-- Dashboard Service -->
          <div class="service-card dashboard-card">
            <div class="service-image">
              <img 
                src="https://images.unsplash.com/photo-1551288049-bebda4e38f71?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3" 
                alt="Dashboard Analytics"
              />
              <div class="service-overlay">
                <BarChart3 :size="32" />
              </div>
            </div>
            <div class="service-content">
              <h3>Operations Dashboard</h3>
              <p>Real-time monitoring and analytics for complete operational visibility</p>
              <ul class="service-features">
                <li><CheckCircle :size="14" /> Live Performance Metrics</li>
                <li><CheckCircle :size="14" /> Predictive Analytics</li>
                <li><CheckCircle :size="14" /> Custom Reporting</li>
              </ul>
              <button @click="navigateTo('dashboard')" class="service-link">
                Explore Dashboard <ArrowRight :size="14" />
              </button>
            </div>
          </div>

          <!-- Container Management Service -->
          <div class="service-card container-card">
            <div class="service-image">
              <img 
                src="https://images.unsplash.com/photo-1566576912321-d58ddd7a6088?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3" 
                alt="Container Port"
              />
              <div class="service-overlay">
                <Container :size="32" />
              </div>
            </div>
            <div class="service-content">
              <h3>Container Management</h3>
              <p>Complete container lifecycle tracking and inventory management</p>
              <ul class="service-features">
                <li><CheckCircle :size="14" /> GPS Location Tracking</li>
                <li><CheckCircle :size="14" /> Inventory Control</li>
                <li><CheckCircle :size="14" /> Automated Logistics</li>
              </ul>
              <button @click="navigateTo('container-management')" class="service-link">
                Manage Containers <ArrowRight :size="14" />
              </button>
            </div>
          </div>

          <!-- Port Operations Service -->
          <div class="service-card operations-card">
            <div class="service-image">
              <img 
                src="https://images.unsplash.com/photo-1578662996442-48f60103fc96?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3" 
                alt="Ship Operations"
              />
              <div class="service-overlay">
                <Anchor :size="32" />
              </div>
            </div>
            <div class="service-content">
              <h3>Port Operations</h3>
              <p>Intelligent vessel scheduling and berth management systems</p>
              <ul class="service-features">
                <li><CheckCircle :size="14" /> Berth Scheduling</li>
                <li><CheckCircle :size="14" /> Resource Allocation</li>
                <li><CheckCircle :size="14" /> Cargo Coordination</li>
              </ul>
              <button @click="navigateTo('port-operation-management')" class="service-link">
                Port Operations <ArrowRight :size="14" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Technology Section -->
    <section class="technology-section">
      <div class="container">
        <div class="technology-content">
          <div class="technology-text">
            <div class="section-badge">
              <Globe :size="16" />
              <span>Advanced Technology</span>
            </div>
            <h2>Built for Modern Ports</h2>
            <p>
              Our platform leverages cutting-edge technology to deliver unparalleled 
              performance, security, and scalability for port operations of any size.
            </p>
            
            <div class="tech-features">
              <div class="tech-feature">
                <div class="tech-icon">
                  <Shield :size="20" />
                </div>
                <div>
                  <h4>Enterprise Security</h4>
                  <p>Bank-level encryption and role-based access control</p>
                </div>
              </div>
              
              <div class="tech-feature">
                <div class="tech-icon">
                  <Cloud :size="20" />
                </div>
                <div>
                  <h4>Cloud-Native Architecture</h4>
                  <p>Scalable, reliable, and always available infrastructure</p>
                </div>
              </div>
              
              <div class="tech-feature">
                <div class="tech-icon">
                  <Database :size="20" />
                </div>
                <div>
                  <h4>Real-Time Data Processing</h4>
                  <p>Instant updates and notifications across all systems</p>
                </div>
              </div>
            </div>
            
            <button @click="navigateTo('event-streaming')" class="btn-outline">
              View Live Events <Activity :size="16" />
            </button>
          </div>
          
          <div class="technology-visual">
            <img 
              src="https://images.unsplash.com/photo-1558618666-fcd25c85cd64?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3" 
              alt="Technology Dashboard"
              class="tech-image"
            />
            <div class="tech-stats">
              <div class="stat-item">
                <div class="stat-value">99.9%</div>
                <div class="stat-label">Uptime</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">&lt;100ms</div>
                <div class="stat-label">Response Time</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">24/7</div>
                <div class="stat-label">Support</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Contact Section -->
    <section class="contact-section">
      <div class="container">
        <div class="contact-content">
          <div class="contact-info">
            <h2>Ready to Transform Your Port Operations?</h2>
            <p>
              Join leading ports worldwide who trust PortTrack for their mission-critical operations. 
              Get started today with a personalized demo.
            </p>
            
            <div class="contact-details">
              <div class="contact-item">
                <Phone :size="20" />
                <div>
                  <h4>Call Us</h4>
                  <p>+1 (555) 123-4567</p>
                </div>
              </div>
              
              <div class="contact-item">
                <Mail :size="20" />
                <div>
                  <h4>Email</h4>
                  <p>info@porttrack.com</p>
                </div>
              </div>
            </div>
          </div>
          
          <div class="contact-form">
            <form @submit.prevent="handleContactForm" class="form">
              <div class="form-group">
                <input 
                  v-model="contactForm.name" 
                  type="text" 
                  placeholder="Full Name" 
                  required
                  class="form-input"
                />
              </div>
              <div class="form-group">
                <input 
                  v-model="contactForm.email" 
                  type="email" 
                  placeholder="Email Address" 
                  required
                  class="form-input"
                />
              </div>
              <div class="form-group">
                <input 
                  v-model="contactForm.company" 
                  type="text" 
                  placeholder="Company/Port Name" 
                  class="form-input"
                />
              </div>
              <div class="form-group">
                <textarea 
                  v-model="contactForm.message" 
                  placeholder="Tell us about your requirements..." 
                  rows="4"
                  class="form-textarea"
                ></textarea>
              </div>
              <button type="submit" class="btn-primary full-width">
                <span>Schedule Demo</span>
                <Calendar :size="18" />
              </button>
            </form>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { 
  Ship, 
  Anchor, 
  ArrowRight, 
  Play, 
  Container, 
  BarChart3, 
  CheckCircle, 
  Activity,
  Zap,
  Clock,
  Globe,
  Database,
  Shield,
  Cloud,
  Phone,
  Mail,
  Calendar
} from 'lucide-vue-next'
import { useToast } from '../../composables/useToast'

const router = useRouter()
const { success, info } = useToast()

// Performance metrics for hero section
const performanceMetrics = ref([
  {
    value: '15K+',
    label: 'Containers Tracked',
    icon: Container,
    color: '#3B82F6'
  },
  {
    value: '99.9%',
    label: 'System Uptime',
    icon: Activity,
    color: '#10B981'
  },
  {
    value: '750+',
    label: 'Vessels Managed',
    icon: Ship,
    color: '#8B5CF6'
  },
  {
    value: '24/7',
    label: 'Live Monitoring',
    icon: Clock,
    color: '#F59E0B'
  }
])

// Contact form data
const contactForm = ref({
  name: '',
  email: '',
  company: '',
  message: ''
})

// Navigation methods
const navigateToDashboard = () => {
  router.push('/dashboard')
}

const navigateTo = (route) => {
  router.push(`/${route}`)
}

// Contact form handler
const handleContactForm = () => {
  success('Thank you for your interest! We\'ll be in touch within 24 hours.')
  info('A demo invitation will be sent to your email shortly.')
  
  // Reset form
  contactForm.value = {
    name: '',
    email: '',
    company: '',
    message: ''
  }
}
</script>

<style scoped>
/* Container and Layout */
.home-container {
  min-height: 100vh;
  background: #ffffff;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

/* Hero Section */
.hero-section {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  overflow: hidden;
}

.hero-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1;
}

.hero-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}

.hero-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    135deg,
    rgba(15, 23, 42, 0.9) 0%,
    rgba(30, 41, 59, 0.8) 50%,
    rgba(51, 65, 85, 0.7) 100%
  );
  z-index: 2;
}

.hero-content {
  position: relative;
  z-index: 3;
  width: 100%;
  padding: 2rem 0;
}

.hero-text {
  max-width: 800px;
  margin-bottom: 4rem;
}

.brand-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(59, 130, 246, 0.1);
  color: #60A5FA;
  padding: 0.5rem 1rem;
  border-radius: 2rem;
  border: 1px solid rgba(59, 130, 246, 0.2);
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 2rem;
  backdrop-filter: blur(10px);
}

.hero-title {
  font-size: clamp(2.5rem, 5vw, 4rem);
  font-weight: 700;
  line-height: 1.1;
  color: #ffffff;
  margin-bottom: 1.5rem;
}

.title-highlight {
  color: #60A5FA;
  display: block;
}

.hero-subtitle {
  font-size: 1.25rem;
  line-height: 1.6;
  color: #CBD5E1;
  margin-bottom: 2.5rem;
  max-width: 600px;
}

.hero-actions {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

/* Button Styles */
.btn-primary {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: #3B82F6;
  color: white;
  padding: 1rem 2rem;
  border-radius: 0.75rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.btn-primary:hover {
  background: #2563EB;
  transform: translateY(-2px);
  box-shadow: 0 8px 25px -8px rgba(59, 130, 246, 0.6);
}

.btn-secondary {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  padding: 1rem 2rem;
  border-radius: 0.75rem;
  font-weight: 600;
  border: 1px solid rgba(255, 255, 255, 0.2);
  cursor: pointer;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.btn-secondary:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.btn-outline {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: transparent;
  color: #3B82F6;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 600;
  border: 2px solid #3B82F6;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-outline:hover {
  background: #3B82F6;
  color: white;
}

/* Metrics Grid */
.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-top: 2rem;
}

.metric-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 1.5rem;
  border-radius: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.metric-card:hover {
  transform: translateY(-4px);
  background: rgba(255, 255, 255, 0.15);
}

.metric-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.metric-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
}

.metric-label {
  font-size: 0.875rem;
  color: #CBD5E1;
}

/* Services Section */
.services-section {
  padding: 6rem 0;
  background: #F8FAFC;
}

.section-header {
  text-align: center;
  margin-bottom: 4rem;
}

.section-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: #EFF6FF;
  color: #3B82F6;
  padding: 0.5rem 1rem;
  border-radius: 2rem;
  font-size: 0.875rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #1E293B;
  margin-bottom: 1rem;
}

.section-subtitle {
  font-size: 1.125rem;
  color: #64748B;
  max-width: 600px;
  margin: 0 auto;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
}

/* Service Cards */
.service-card {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.service-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.service-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.service-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.service-card:hover .service-image img {
  transform: scale(1.05);
}

.service-overlay {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 3rem;
  height: 3rem;
  background: rgba(0, 0, 0, 0.7);
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  backdrop-filter: blur(10px);
}

.service-content {
  padding: 1.5rem;
}

.service-content h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1E293B;
  margin-bottom: 0.5rem;
}

.service-content p {
  color: #64748B;
  margin-bottom: 1rem;
  line-height: 1.6;
}

.service-features {
  list-style: none;
  padding: 0;
  margin: 0 0 1.5rem 0;
}

.service-features li {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #475569;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.service-features svg {
  color: #10B981;
}

.service-link {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: #3B82F6;
  font-weight: 500;
  background: none;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.service-link:hover {
  color: #2563EB;
  gap: 0.75rem;
}

/* Technology Section */
.technology-section {
  padding: 6rem 0;
  background: white;
}

.technology-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
}

.technology-text h2 {
  font-size: 2rem;
  font-weight: 700;
  color: #1E293B;
  margin-bottom: 1rem;
}

.technology-text p {
  font-size: 1.125rem;
  color: #64748B;
  line-height: 1.6;
  margin-bottom: 2rem;
}

.tech-features {
  margin-bottom: 2rem;
}

.tech-feature {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.tech-icon {
  width: 2.5rem;
  height: 2.5rem;
  background: #EFF6FF;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #3B82F6;
  flex-shrink: 0;
}

.tech-feature h4 {
  font-weight: 600;
  color: #1E293B;
  margin-bottom: 0.25rem;
}

.tech-feature p {
  color: #64748B;
  font-size: 0.875rem;
  margin: 0;
}

.technology-visual {
  position: relative;
}

.tech-image {
  width: 100%;
  border-radius: 1rem;
  box-shadow: 0 10px 40px -10px rgba(0, 0, 0, 0.2);
}

.tech-stats {
  position: absolute;
  bottom: -2rem;
  right: -2rem;
  background: white;
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: 0 10px 40px -10px rgba(0, 0, 0, 0.2);
  display: flex;
  gap: 1.5rem;
}

.stat-item {
  text-align: center;
}

.stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1E293B;
}

.stat-label {
  font-size: 0.75rem;
  color: #64748B;
  margin-top: 0.25rem;
}

/* Contact Section */
.contact-section {
  padding: 6rem 0;
  background: #1E293B;
  color: white;
}

.contact-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: start;
}

.contact-info h2 {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.contact-info p {
  font-size: 1.125rem;
  color: #CBD5E1;
  line-height: 1.6;
  margin-bottom: 2rem;
}

.contact-details {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.contact-item svg {
  color: #60A5FA;
}

.contact-item h4 {
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.contact-item p {
  color: #CBD5E1;
  margin: 0;
}

/* Form Styles */
.form {
  background: rgba(255, 255, 255, 0.05);
  padding: 2rem;
  border-radius: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-input,
.form-textarea {
  width: 100%;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.5rem;
  color: white;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.form-input::placeholder,
.form-textarea::placeholder {
  color: #94A3B8;
}

.form-input:focus,
.form-textarea:focus {
  outline: none;
  border-color: #60A5FA;
  background: rgba(255, 255, 255, 0.15);
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

.full-width {
  width: 100%;
  justify-content: center;
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-actions {
    flex-direction: column;
    width: 100%;
  }
  
  .hero-actions button {
    width: 100%;
    justify-content: center;
  }
  
  .metrics-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .services-grid {
    grid-template-columns: 1fr;
  }
  
  .technology-content,
  .contact-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
  
  .tech-stats {
    position: static;
    margin-top: 1rem;
    justify-content: space-around;
  }
  
  .section-title {
    font-size: 2rem;
  }
  
  .hero-title {
    font-size: 2.5rem;
  }
}

@media (max-width: 480px) {
  .metrics-grid {
    grid-template-columns: 1fr;
  }
  
  .services-grid {
    grid-template-columns: 1fr;
  }
  
  .metric-card {
    flex-direction: column;
    text-align: center;
  }
  
  .container {
    padding: 0 1rem;
  }
}

/* Animation Classes */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeInUp 0.6s ease-out;
}
</style>