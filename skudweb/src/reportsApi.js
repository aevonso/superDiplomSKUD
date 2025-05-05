import apiClient from './apiClient';

/**
 * Генерация отчёта в заданном формате.
 * @param {'pdf'|'docx'} format
 * @returns Blob
 */
export async function generateReport(format, options) {
    const resp = await apiClient.post(
        '/api/reports',
        {
            includeEmployees: options.includeEmployees,
            includeMobileDevices: options.includeMobileDevices,
            includeAccessAttempts: options.includeAccessAttempts,
            format: format
        },
        { responseType: 'blob' }
    );
    return resp.data;
}