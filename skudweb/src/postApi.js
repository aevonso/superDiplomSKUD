import apiClient from './apiClient';

export async function fetchPosts() {
    const resp = await apiClient.get('/api/Post');
    return resp.data; // [{ id, name }, …]
}
