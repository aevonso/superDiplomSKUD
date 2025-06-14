import apiClient from './apiClient';

/**
 * Генерация отчёта в заданном формате (PDF или DOCX).
 * @param {'pdf'|'docx'} format
 * @param {Object} options - Опции для формирования отчёта
 * @returns {Blob} - Blob с отчётом
 */
export async function generateReport(format, options) {
    // Преобразуем строки дат в объекты Date перед отправкой
    const fromDate = options.fromDate ? new Date(options.fromDate) : null;
    const toDate = options.toDate ? new Date(options.toDate) : null;

    const resp = await apiClient.post(
        '/api/reports',
        {
            includeAccessAttempts: options.includeAccessAttempts,
            successOnly: options.successOnly,
            failedOnly: options.failedOnly,
            fromDate: fromDate ? fromDate.toISOString() : null, // Преобразуем в ISO формат
            toDate: toDate ? toDate.toISOString() : null, // Преобразуем в ISO формат
            format: format
        },
        { responseType: 'blob' }
    );
    return resp.data;
}
