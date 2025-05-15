import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'https://localhost:7267',
    headers: { 'Content-Type': 'application/json' }
});

apiClient.interceptors.request.use(cfg => {
    const token = localStorage.getItem('accessToken');
    if (token) cfg.headers.Authorization = `Bearer ${token}`;
    return cfg;
});

export default apiClient;
