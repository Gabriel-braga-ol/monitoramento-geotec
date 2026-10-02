import axios from 'axios';

// Aponta para a API do backend
export const api = axios.create({
    baseURL: 'http://localhost:5196/api',
});

