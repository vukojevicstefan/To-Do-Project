import React, { useState } from "react";
import "../css/TodoList.css";
import TaskItem from "./TaskItem";

const TodoList = ({ todoList, setToDoLists }) => {
  const [taskItems, setTaskItems] = useState(todoList.taskItems || []);
  const [editingListName, setEditingListName] = useState(false);
  const [newListName, setNewListName] = useState(todoList.name);

  const addTask = async (taskName, description = "", time = null) => {
    try {
      const response = await fetch(
        `https://localhost:7046/ToDoLists/AddTaskToToDo/${todoList.id}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({ name: taskName, description, time }),
        }
      );

      if (response.ok) {
        const newTask = await response.json();
        setTaskItems((prev) => [...prev, newTask]);
      } else {
        console.error("Failed to add task:", await response.text());
      }
    } catch (err) {
      console.error(err);
    }
  };

  const deleteToDoList = async (id) => {
    try {
      const response = await fetch(
        `https://localhost:7046/ToDoLists/DeleteToDoList/${id}`,
        { method: "DELETE" }
      );
      if (response.ok) setToDoLists((prev) => prev.filter((list) => list.id !== id));
    } catch (err) {
      console.error("Failed to delete To Do List:", err);
    }
  };

  const handleUpdateTaskName = (taskId, newName) => {
    setTaskItems((prev) =>
      prev.map((task) => (task.id === taskId ? { ...task, name: newName } : task))
    );
  };

  const handleToggleChecked = (taskId) => {
    setTaskItems((prev) =>
      prev.map((task) =>
        task.id === taskId ? { ...task, checked: !task.checked } : task
      )
    );
  };

  const handleRemoveTask = (taskId) => {
    setTaskItems((prev) => prev.filter((task) => task.id !== taskId));
  };

    const updateToDoListName = async (id, name) => {
    try {
      const encodedName = encodeURIComponent(name);
      const response = await fetch(
        `https://localhost:7046/ToDoLists/ChangeToDoListName/${id}/${encodedName}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (response.ok) {
        setToDoLists((prevLists) =>
          prevLists.map((list) =>
            list.id === id ? { ...list, name: name } : list
          )
        );
      } else {
        const error = await response.text();
        console.error("Failed to update ToDoList name:", error);
      }
    } catch (err) {
      console.error("Error updating ToDoList name:", err);
    }
  }

  return (
    <div className="todo-list">
      <button
        className="delete-list-button"
        onClick={() => deleteToDoList(todoList.id)}
      >
        ×
      </button>

      {editingListName ? (
        <input
          className="edit-todolist-input"
          type="text"
          value={newListName}
          onChange={(e) => setNewListName(e.target.value)}
          onBlur={() => {
            setEditingListName(false);
            setNewListName(todoList.name); // reset if cancelled
          }}
          onKeyDown={(e) => {
            if (e.key === "Enter" && newListName.trim() !== "") {
              updateToDoListName(todoList.id, newListName.trim());
              setEditingListName(false);
            }
          }}
          autoFocus
        />
      ) : (
        <h2
          className="todo-list-name"
          onDoubleClick={() => {
            setEditingListName(true);
          }}
        >
          {todoList.name}
        </h2>
      )}
      <div className="red-dotted-line"></div>

      <ul>
        {taskItems.map((task) => (
          <TaskItem
            key={task.id}
            task={task}
            onRemove={handleRemoveTask}
            onUpdateName={handleUpdateTaskName}
            onToggleChecked={handleToggleChecked}
          />
        ))}
      </ul>

      <li>
        <input
          type="text"
          placeholder="Add new task"
          onKeyDown={(e) => {
            if (e.key === "Enter" && e.target.value.trim() !== "") {
              addTask(e.target.value.trim());
              e.target.value = "";
            }
          }}
        />
      </li>
    </div>
  );
};

export default TodoList;
