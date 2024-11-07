import React, { useState } from "react";
import { createTodo, updateTodo } from "@services/api";
import { TodoItemReadDto } from "types/Todo";

interface Props {
  todo?: TodoItemReadDto;
  onSuccess: (data: unknown) => void;
  onClose?: () => void;
}

const TodoForm: React.FC<Props> = ({ todo, onSuccess, onClose }) => {
  const [title, setTitle] = useState<string | null>(todo ? todo.title : "");
  const [description, setDescription] = useState<string>(
    todo ? todo.description || "" : ""
  );
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title?.trim()) {
      setError("Title is required.");
      return;
    }

    try {
      if (todo) {
        const response = await updateTodo(todo.id, {
          ...todo,
          title,
          description,
        });
        onSuccess(response);
      } else {
        const response = await createTodo({
          title,
          description,
        });
        onSuccess(response);
      }
      setTitle("");
      setDescription("");
      setError(null);
      if (onClose) {
        onClose();
      }
    } catch (err) {
      setError("Failed to save TODO item.");
      console.debug(err);
    }
  };

  return (
    <div className="flex justify-center">
      <form
        onSubmit={handleSubmit}
        className="shadow-md p-4 rounded-lg min-w-96 my-10 bg-gray-800"
      >
        <h2 className="text-xl font-bold mb-2 text-center">
          {todo ? "Edit TODO" : "Add New TODO"}
        </h2>
        {error && <p className="text-red-500 mb-2">{error}</p>}
        <div className="mb-2">
          <label className="block text-gray-200">Title *</label>
          <input
            type="text"
            placeholder="Enter title"
            className="w-full border rounded px-3 py-2 bg-gray-800"
            value={title ?? ""}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>
        <div className="mb-2">
          <label className="block text-gray-200">Description</label>
          <textarea
            placeholder="Optional"
            className="w-full border rounded px-3 py-2 bg-gray-800"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          ></textarea>
        </div>
        <div className="flex justify-end space-x-2">
          {onClose && (
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-white rounded hover:bg-gray-600"
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            className="px-6 py-1 bg-purple-700 text-white rounded-full hover:bg-purple-600"
          >
            {todo ? "Update" : "Add"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default TodoForm;
