import apiClient from './apiClient';

/** Сотрудники */
export async function fetchEmployees(params = {}) {
    const q = new URLSearchParams();
    if (params.fullName) q.append('fullName', params.fullName);
    if (params.phone) q.append('phone', params.phone);
    if (params.login) q.append('login', params.login);
    if (params.divisionId != null) q.append('divisionId', params.divisionId);
    if (params.postId != null) q.append('postId', params.postId);
    q.append('take', String(params.take ?? 100));
    const resp = await apiClient.get(`/api/Employee?${q}`);
    return resp.data;
}

export async function fetchEmployeeById(id) {
    const resp = await apiClient.get(`/api/Employee/${id}`);
    return resp.data;
}

export async function createEmployee(emp) {
    const resp = await apiClient.post('/api/Employee', emp);
    return resp.data;
}

export async function updateEmployee(id, emp) {
    await apiClient.put(`/api/Employee/${id}`, emp);
}

export async function deleteEmployee(id) {
    await apiClient.delete(`/api/Employee/${id}`);
}

// — Аватарки
export async function uploadAvatar(id, file) {
    const form = new FormData();
    form.append('file', file);
    await apiClient.put(`/api/Employee/${id}/avatar`, form, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
}

export async function deleteAvatar(id) {
    await apiClient.delete(`/api/Employee/${id}/avatar`);
}

export function getAvatarUrl(id) {
    return `${apiClient.defaults.baseURL}/api/Employee/${id}/avatar`;
}
