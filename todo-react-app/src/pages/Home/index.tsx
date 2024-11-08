import React, { useCallback } from "react";
import { useNavigate } from "react-router-dom";
import TodoList from "@components/TodoList";

const Home: React.FC = () => {
  const navigate = useNavigate();

  const handleAddTodo = useCallback(() => {
    navigate("/add");
  }, [navigate]);

  return (
    <div className="flex justify-start items-center flex-col m-8  rounded-lg">
      <h1 className="p-4 text-2xl font-bold">TODO List</h1>
      <div className="container mx-auto p-4 flex justify-end">
        <button
          onClick={handleAddTodo}
          className="w-full md:w-auto bg-purple-700 hover:bg-purple-900 text-white font-thin py-2 px-4 rounded-full"
        >
          + Add New Todo
        </button>
      </div>

      <TodoList />
    </div>
  );
};

export default Home;
