import apiClient from './apiClient';

export async function fetchDivisions() {
    const resp = await apiClient.get('/api/Division');
    return resp.data; // [{ id, name }, …]
}
