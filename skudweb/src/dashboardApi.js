import apiClient from './apiClient';


export async function fetchStats() {
    const resp = await apiClient.get('/api/Dashboard/stats');
    return resp.data;  
}

/**
 * Получить попытки с фильтрами
 * @param {{ from?: string, to?: string, pointId?: number, employeeId?: number, take?: number }} params
 */
export async function fetchAttempts(params = {}) {
    const q = new URLSearchParams();
    if (params.from) q.append('from', params.from);
    if (params.to) q.append('to', params.to);
    if (params.pointId) q.append('pointId', String(params.pointId));
    if (params.employeeId) q.append('employeeId', String(params.employeeId));
    q.append('take', String(params.take ?? 10));

    const resp = await apiClient.get(`/api/Dashboard/attempts?${q.toString()}`);
    return resp.data;  // Array<AttemptDto>
}
