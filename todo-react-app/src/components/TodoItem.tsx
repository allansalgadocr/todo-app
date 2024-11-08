import React from "react";
import { TodoItemReadDto } from "types/Todo";
import { BiCheckDouble, BiPencil, BiX } from "react-icons/bi";

interface Props {
  todo: TodoItemReadDto;
  onDelete: (id: string) => void;
  onEdit: (todo: TodoItemReadDto) => void;
  onToggleComplete: (id: string) => void; // Nueva función para cambiar el estado
}

const TodoItem: React.FC<Props> = ({
  todo,
  onDelete,
  onEdit,
  onToggleComplete,
}) => {
  return (
    <li className="flex flex-col md:flex-row justify-center md:justify-between items-start md:items-center bg-gradient-to-b from-gray-600 to-gray-900 shadow-md p-4 mb-2 rounded-lg min-h-[85px]">
      <div className="flex items-center space-x-3">
        {/* custom checkbox */}
        <label className="flex items-center cursor-pointer">
          <input
            type="checkbox"
            checked={todo.isCompleted}
            onChange={() => onToggleComplete(todo.id)}
            className="hidden peer"
          />
          <div className="w-6 h-6 border-2 border-red-400 rounded-md flex items-center justify-center peer-checked:bg-green-600 peer-checked:border-green-600">
            {todo.isCompleted ? <BiCheckDouble className="text-white" /> : null}
          </div>
        </label>
        {/* title & description */}
        <div>
          <h2
            className={`md:text-xl text-gray-200 ${
              todo.isCompleted ? "line-through" : ""
            }`}
          >
            {todo.title}
          </h2>
          <p className="text-sm md:text-base text-gray-200 font-thin">
            {todo.description}
          </p>
        </div>
      </div>
      <div className="flex self-end md:self-center space-x-2">
        <button onClick={() => onEdit(todo)} className="">
          <BiPencil className="text-gray-200 hover:text-gray-200 text-2xl" />
        </button>
        <button onClick={() => onDelete(todo.id)} className="">
          <BiX className="text-red-600 hover:text-red-800 text-2xl" />
        </button>
        <button
          onClick={() => onToggleComplete(todo.id)}
          className={`px-3 py-1 rounded ${
            todo.isCompleted ? "bg-green-500" : "bg-yellow-500"
          } text-white focus:outline-none focus:ring-2 focus:ring-green-300 transition duration-200`}
          aria-label={
            todo.isCompleted ? "Mark as Incompleted" : "Mark as Completed"
          }
        >
          {todo.isCompleted ? "Undo" : "Complete"}
        </button>
      </div>
    </li>
  );
};

export default TodoItem;
