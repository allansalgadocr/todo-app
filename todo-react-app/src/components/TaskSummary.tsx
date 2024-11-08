interface TaskSummaryProps {
  incompletedItems: number;
  totalTasks: number;
}

const TaskSummary: React.FC<TaskSummaryProps> = ({
  incompletedItems,
  totalTasks,
}) => (
  <p className="font-thing text-sm text-center md:text-left">
    You have{" "}
    <span className={incompletedItems > 0 ? "text-red-500" : "text-green-500"}>
      {incompletedItems}
    </span>{" "}
    incomplete task
    {incompletedItems !== 1 && "s"} of {totalTasks}
  </p>
);

export default TaskSummary;
