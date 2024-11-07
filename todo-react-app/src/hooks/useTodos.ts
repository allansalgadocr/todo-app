import { useState, useEffect, useCallback, useMemo } from "react";
import { debounce } from "lodash";
import { getTodos, deleteTodo, updateTodo } from "@services/api";
import { TodoItemReadDto } from "types/Todo";
import { useNavigate } from "react-router-dom";

interface UseTodosReturn {
  todos: TodoItemReadDto[];
  loading: boolean;
  error: string | null;
  handleDelete: (id: string) => Promise<void>;
  handleEdit: (todo: TodoItemReadDto) => void;
  handleToggleComplete: (id: string) => Promise<void>;
}

const useTodos = (): UseTodosReturn => {
  const navigate = useNavigate();

  const [todos, setTodos] = useState<TodoItemReadDto[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Centralized error handler
  const handleError = useCallback((action: string, error: any) => {
    console.error(`Error during ${action}:`, error);

    if (error.response) {
      // Server responded with a status other than 2xx
      const message =
        error.response.data.message || "An unexpected server error occurred.";
      setError(`Server Error: ${message}`);
    } else if (error.request) {
      // Request was made but no response received
      setError("Network Error: Please check your internet connection.");
    } else {
      // Something else happened
      setError(`Error: ${error.message}`);
    }
  }, []);

  // Fetch all todos with debounce to prevent excessive calls
  const fetchTodos = useMemo(
    () =>
      debounce(async () => {
        setLoading(true);
        setError(null);
        try {
          const response = await getTodos();
          setTodos(response);
        } catch (err: any) {
          handleError("fetch", err);
        } finally {
          setLoading(false);
        }
      }, 300),
    [handleError]
  );

  useEffect(() => {
    fetchTodos();
    return () => {
      fetchTodos.cancel();
    };
  }, [fetchTodos]);

  const handleDelete = useCallback(
    async (id: string) => {
      try {
        // Optimistically remove the todo from UI
        setTodos((prevTodos) => prevTodos.filter((todo) => todo.id !== id));
        await deleteTodo(id);
      } catch (err: any) {
        handleError("delete", err);
        // Re-fetch the todos to ensure UI is in sync with server
        fetchTodos();
      }
    },
    [fetchTodos, handleError]
  );

  const handleEdit = useCallback(
    (todo: TodoItemReadDto) => {
      navigate(`/edit/${todo.id}`);
    },
    [navigate]
  );

  const handleToggleComplete = useCallback(
    async (id: string) => {
      const todo = todos.find((todo) => todo.id === id);
      if (!todo) return;

      const updatedTodo: TodoItemReadDto = {
        ...todo,
        isCompleted: !todo.isCompleted,
      };

      // Optimistically update the todo
      setTodos((prevTodos) =>
        prevTodos.map((t) => (t.id === id ? updatedTodo : t))
      );

      try {
        await updateTodo(id, {
          title: updatedTodo.title || "",
          description: updatedTodo.description || "",
          isCompleted: updatedTodo.isCompleted,
        });
      } catch (err: any) {
        handleError("update", err);
        // Revert the optimistic update
        setTodos((prevTodos) => prevTodos.map((t) => (t.id === id ? todo : t)));
      }
    },
    [todos, handleError]
  );

  return {
    todos,
    loading,
    error,
    handleDelete,
    handleEdit,
    handleToggleComplete,
  };
};

export default useTodos;
