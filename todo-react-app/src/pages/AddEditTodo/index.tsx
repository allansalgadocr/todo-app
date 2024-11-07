import React, { useEffect, useState, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import TodoForm from "@components/TodoForm";
import { getTodo } from "@services/api";
import { TodoItemReadDto } from "types/Todo";

const AddEditTodo = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [editingTodo, setEditingTodo] = useState<TodoItemReadDto | null>(null);

  // Handlers with useCallback to prevent unnecessary re-creations
  const handleSuccess = useCallback(() => {
    navigate("/");
  }, [navigate]);

  const handleCancel = useCallback(() => {
    navigate("/");
  }, [navigate]);

  useEffect(() => {
    const fetchTodoDetail = async (id: string) => {
      try {
        setLoading(true);
        const todo = await getTodo(id);
        setEditingTodo(todo);
      } catch (err) {
        setError("Failed to fetch the TODO item. Please try again.");
        console.debug(err);
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchTodoDetail(id);
    }
  }, [id]);

  if (loading) return <p className="text-white text-center mt-4">Loading...</p>;
  if (error) return <p className="text-center mt-4 text-red-500">{error}</p>;

  return (
    <TodoForm
      todo={editingTodo || undefined}
      onClose={handleCancel}
      onSuccess={handleSuccess}
    />
  );
};

export default AddEditTodo;
