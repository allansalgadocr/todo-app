import axios, { AxiosInstance } from "axios";
import {
  TodoItemCreateDto,
  TodoItemReadDto,
  TodoItemUpdateDto,
} from "../types/Todo";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;
const API_KEY = import.meta.env.VITE_API_KEY;

// Validate environment variables
if (!BASE_URL) {
  throw new Error(
    "REACT_APP_API_BASE_URL is not defined in the environment variables."
  );
}

if (!API_KEY) {
  throw new Error(
    "REACT_APP_API_KEY is not defined in the environment variables."
  );
}

const api: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  headers: {
    "X-API-KEY": API_KEY,
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export const getTodos = async (): Promise<TodoItemReadDto[]> => {
  const response = await api.get<TodoItemReadDto[]>("/Todos");
  return response.data;
};

export const getTodo = async (id: string): Promise<TodoItemReadDto> => {
  const response = await api.get<TodoItemReadDto>(`/Todos/${id}`);
  return response.data;
};

export const createTodo = async (
  todo: TodoItemCreateDto
): Promise<TodoItemReadDto> => {
  const response = await api.post<TodoItemReadDto>("/Todos", todo);
  return response.data;
};

export const updateTodo = async (
  id: string,
  todo: TodoItemUpdateDto
): Promise<void> => {
  await api.put(`/Todos/${id}`, todo);
};

export const deleteTodo = async (id: string): Promise<void> => {
  await api.delete(`/Todos/${id}`);
};

export default api;
