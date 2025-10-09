<script setup lang="ts">
import { ref, reactive } from 'vue'
import {
  Ship,
  User,
  Lock,
  LogIn,
  Loader2,
  AlertTriangle,
  CheckCircle,
  ArrowLeft,
  Eye,
  EyeOff
} from 'lucide-vue-next'
import { authApi } from '../services/api'
import { useToast } from '../composables/useToast.js'

const emit = defineEmits<{
  (e: 'login-success', payload: any): void
  (e: 'show-register'): void
  (e: 'cancel'): void
}>()

const { success, error: showError } = useToast()

interface LoginForm {
  username: string
  password: string
  rememberMe: boolean
}

const form = reactive<LoginForm>({
  username: '',
  password: '',
  rememberMe: false
})

const errors = reactive<Partial<Record<keyof LoginForm, string>>>({})
const showPassword = ref(false)
const isLoading = ref(false)

const validate = (): boolean => {
  // clear errors
  Object.keys(errors).forEach(k => delete errors[k as keyof LoginForm])

  if (!form.username.trim()) {
    errors.username = 'Username is required'
  }

  if (!form.password.trim()) {
    errors.password = 'Password is required'
  }

  return !errors.username && !errors.password
}

const handleSubmit = async () => {
  if (!validate()) return

  isLoading.value = true

  try {
    const loginRequest = {
      username: form.username,
      password: form.password
    }

    // if authApi uses axios:
    const response = await authApi.login(loginRequest)
    const data = response.data ?? response // support both shapes

    success('Login successful! Welcome back! 🎉')

    const currentUser = {
      id: data.user?.userId ?? data.user?.id,
      username: data.user?.username,
      email: data.user?.email,
      fullName: data.user?.fullName,
      roles: data.user?.roles,
      permissions: data.user?.permissions,
      loginTime: new Date().toISOString(),
      token: data.token ?? data.accessToken
    }

    localStorage.setItem('current_user', JSON.stringify(currentUser))
    localStorage.setItem('auth_token', currentUser.token)

    setTimeout(() => {
      emit('login-success', currentUser)
    }, 1000)
  } catch (err: any) {
    console.error('Login error:', err)
    if (err.response?.status === 401) {
      showError('Invalid username or password.')
    } else if (err.response?.data?.message) {
      showError(err.response.data.message)
    } else {
      showError('Login failed. Please try again.')
    }
  } finally {
    isLoading.value = false
  }
}
</script>
