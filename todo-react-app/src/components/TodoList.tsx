import React, { useState, useMemo } from "react";
import TodoItem from "./TodoItem";
import TaskSummary from "./TaskSummary";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import useTodos from "../hooks/useTodos";

enum Filter {
  All = "all",
  Completed = "completed",
  Incompleted = "incompleted",
}

const TodoList: React.FC = () => {
  const [filter, setFilter] = useState<Filter>(Filter.All);
  const {
    todos,
    loading,
    error,
    handleDelete,
    handleEdit,
    handleToggleComplete,
  } = useTodos();

  // Memoized filtered todos based on current filter
  const filteredTodos = useMemo(() => {
    return todos.filter((todo) =>
      filter === Filter.Completed
        ? todo.isCompleted
        : filter === Filter.Incompleted
        ? !todo.isCompleted
        : true
    );
  }, [todos, filter]);

  // Row renderer for react-window to virtualize the list
  const Row = ({
    index,
    style,
  }: {
    index: number;
    style: React.CSSProperties;
  }) => {
    const todo = filteredTodos[index];
    return (
      <div style={style}>
        <MemoizedTodoItem
          key={todo.id}
          todo={todo}
          onDelete={handleDelete}
          onEdit={handleEdit}
          onToggleComplete={handleToggleComplete}
        />
      </div>
    );
  };

  if (loading) return <p className="text-white text-center mt-4">Loading...</p>;
  if (error) return <p className="text-center mt-4 text-red-500">{error}</p>;

  return (
    <div className="container mx-auto p-4">
      <div className="flex flex-col md:flex-row justify-between">
        <TaskSummary
          incompletedItems={
            filteredTodos.filter((todo) => !todo.isCompleted).length
          }
          totalTasks={todos.length}
        />

        {/* Filter buttons */}
        <div className="self-center md:self-end flex mt-4 md:mt-0">
          {Object.values(Filter).map((filterValue) => (
            <button
              key={filterValue}
              className={`p-2 px-4 text-xs mx-1 rounded-full ${
                filter === filterValue
                  ? "bg-purple-500 text-white"
                  : "bg-gray-200 text-gray-900"
              }`}
              onClick={() => setFilter(filterValue)}
            >
              {filterValue.charAt(0).toUpperCase() + filterValue.slice(1)}
            </button>
          ))}
        </div>
      </div>

      {/* Virtualized list */}
      {filteredTodos.length === 0 ? (
        <p className="my-4 text-center text-white">No TODO items found.</p>
      ) : (
        <div className="mt-4 h-[600px]">
          <AutoSizer>
            {({ height, width }) => (
              <List
                height={height}
                itemCount={filteredTodos.length}
                itemSize={90}
                width={width}
              >
                {Row}
              </List>
            )}
          </AutoSizer>
        </div>
      )}
    </div>
  );
};

// Memoized TodoItem component to prevent unnecessary re-renders
const MemoizedTodoItem = React.memo(TodoItem);

export default TodoList;
