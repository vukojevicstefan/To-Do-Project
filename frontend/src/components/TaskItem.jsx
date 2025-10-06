import React, { useState } from "react";
import "../css/TaskItem.css";
const TaskItem = ({ task, onRemove, onUpdateName, onToggleChecked }) => {
  const [editing, setEditing] = useState(false);
  const [taskName, setTaskName] = useState(task.name);

  // Toggle task completion
  const toggleTaskCompletion = async () => {
    try {
      const response = await fetch(
        `https://localhost:7046/TaskItems/ToggleTaskItemChecked/${task.id}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (response.ok) {
        onToggleChecked(task.id);
      } else {
        console.error("Failed to toggle task:", await response.text());
      }
    } catch (err) {
      console.error("Error toggling task:", err);
    }
  };

  // Update task name
  const updateTaskName = async () => {
    if (taskName.trim() === "") {
      setTaskName(task.name);
      setEditing(false);
      return;
    }

    try {
      const encodedName = encodeURIComponent(taskName.trim());
      const response = await fetch(
        `https://localhost:7046/TaskItems/ChangeTaskItemName/${task.id}/${encodedName}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (response.ok) {
        onUpdateName(task.id, taskName.trim()); // <-- update parent state
        setEditing(false);
      } else {
        console.error("Failed to update task name:", await response.text());
      }
    } catch (err) {
      console.error("Error updating task name:", err);
    }
  };

  // Delete task
  const deleteTask = async () => {
    try {
      const response = await fetch(
        `https://localhost:7046/TaskItems/DeleteTaskItem/${task.id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (response.ok) {
        onRemove(task.id);
      } else {
        console.error("Failed to delete task:", await response.text());
      }
    } catch (err) {
      console.error("Error deleting task:", err);
    }
  };

  return (
    <li className="task-item">
      {editing ? (
        <input
          type="text"
          className="edit-task-input"
          value={taskName}
          onChange={(e) => setTaskName(e.target.value)}
          onBlur={updateTaskName}
          onKeyDown={(e) => {
            if (e.key === "Enter") updateTaskName();
          }}
          autoFocus
        />
      ) : (
        <span
          onClick={toggleTaskCompletion}
          onDoubleClick={() => setEditing(true)}
          style={{
            textDecoration: task.checked ? "line-through" : "none",
            opacity: task.checked ? 0.6 : 1,
            flexGrow: 1,
            cursor: "pointer",
            transition: "all 0.2s ease",
          }}
        >
          - {task.name} {task.repeat ? "(Repeats)" : ""}
        </span>
      )}

      <button
        className="delete-task-button"
        aria-label="Delete Task"
        onClick={deleteTask}
      >
        ×
      </button>
    </li>
  );
};

export default TaskItem;
