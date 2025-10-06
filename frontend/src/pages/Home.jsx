import React, { useState, useEffect } from "react";
import TodoList from "../components/TodoList";
import Navbar from "../components/Navbar";
import "../css/Home.css";
import AddListModal from "../components/AddListModal";
import Search from "../components/Search";

const Home = ({ setIsAuthenticated }) => {
    const [todoLists, setTodoLists] = useState([]);
    const [filteredToDoLists, setFilteredToDoLists] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);

    // Fetch user's todo lists
    const fetchTodoLists = async () => {
        try {
        const response = await fetch("https://localhost:7046/Users/CurrentUserToDoLists", {
            headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${localStorage.getItem("token")}`
            }
        });

        if (response.ok) {
            const data = await response.json();
            setTodoLists(data);
            setFilteredToDoLists(data);
            console.log(data[0]);
        } else if (response.status === 401) {
            setIsAuthenticated(false);
        }
        } catch (err) {
        console.error(err);
        }
    };

    useEffect(() => {
        fetchTodoLists();
    }, []);

    const handleAddToDoList = async (listName) => {
        try {
        const response = await fetch(
            `https://localhost:7046/ToDoLists/CreateToDoList/${encodeURIComponent(listName)}`,
            {
            method: "POST",
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
            }
        );

        if (response.ok) {
            const newList = await response.json();
            setTodoLists((prev) => [...prev, newList]);
        } else {
            const error = await response.text();
            console.error("Failed to add list:", error);
        }
        } catch (err) {
        console.error(err);
        }
    };

  return (
    <div>
        <Navbar setIsAuthenticated={setIsAuthenticated} />
        <div className={isModalOpen ? "blur-background" : ""}>
            <div className="first-row">

                <button className="add-button" aria-label="Add" onClick={() => setIsModalOpen(true)}>
                +
                </button>
                <Search className="search" toDoLists={todoLists} setToDoLists={setFilteredToDoLists}/>
            </div>
            <div className="todo-lists-container">


                {filteredToDoLists.length === 0 ? (
                <p>No lists.</p>
                ) : (
                filteredToDoLists.map((list) => <TodoList setToDoLists={setFilteredToDoLists} key={list.id} todoList={list} />)
                )}
            </div>

        </div>
            <AddListModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onAdd={handleAddToDoList}
            />
    </div>
  );
};

export default Home;