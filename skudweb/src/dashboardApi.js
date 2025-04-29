import apiClient from './apiClient'

export async function fetchRecentAttempts(take = 10) {
    const resp = await apiClient.get(`/api/Dashboard/attempts?take=${take}`)
    return resp.data
}
