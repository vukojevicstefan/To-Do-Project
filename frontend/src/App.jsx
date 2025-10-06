import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Home from "./pages/Home";
import { useState, useEffect } from "react";
import logo from "./assets/react.svg";
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsAuthenticated(!!token);
  }, []);

  if (isAuthenticated === null) {
    return <img src={logo} alt="Loading..." className="spinner" />;
  }

  return (
    <Routes>
      <Route path="/login" element={<Login setIsAuthenticated={setIsAuthenticated}/>} />
      <Route
        path="/home"
        element={isAuthenticated ? <Home setIsAuthenticated={setIsAuthenticated}/> : <Navigate to="/login" />}
      />
      <Route path="*" element={<Navigate to={isAuthenticated ? "/home" : "/login"} />} />
    </Routes>
  );
}

export default App;
