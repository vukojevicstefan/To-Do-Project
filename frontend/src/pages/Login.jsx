import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../css/Login.css";

const Login = ({setIsAuthenticated}) => {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [username, setUsername] = useState("");

    const [isLogin, setIsLogin] = useState(true);
    const [isRegister, setIsRegister] = useState(false);

    const [error, setError] = useState(""); // <-- new state for error messages

    const handleLogin = async (e) => {
        e.preventDefault();
        setError(""); // reset previous error

        if (!email || !password) {
        setError("Please enter email and password");
        return;
        }

        try {
            const body = JSON.stringify({ email, password });
                const response = await fetch(`https://localhost:7046/Login/Login`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: body,
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.setItem("token", data.token);
                    setIsAuthenticated(true);
                    navigate("/home"); // redirect to Home
                } else {
                    const errorText = await response.text();
                    setError(errorText);
                }
            } catch (err) {
            console.error(err);
            setError("An error occurred while logging in. Please try again");
            }
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setError("");

        if (!username || !email || !password || !confirmPassword) {
        setError("Please fill in all fields");
        return;
        }

        if (password !== confirmPassword) {
        setError("Passwords do not match");
        return;
        }

        try {
            const body = JSON.stringify({ username, email, password, confirmPassword });
            const response = await fetch(`https://localhost:7046/Login/SignUp`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: body,
            });

            if (response.ok) {
                console.log("Status:", response.status);
                setError("Registration successful! Please log in.");
                emptyStates();
                switchLR(true);
            } else {
                const errorText = await response.text();
                setError(errorText);
            }
            } catch (err) {
                console.error(err);
                setError("An error occurred while registering.");
            }
    };

    const switchLR = (loginBool) => {
        setIsLogin(loginBool);
        setIsRegister(!loginBool);
        setError(""); // clear error when switching forms
    };
    const emptyStates = () => {
        setEmail("");
        setPassword("");
        setConfirmPassword("");
        setUsername("");
    };

    return (
    <div className="login-page">
      <div className="left">
        <h1>Welcome!</h1>
        <p>Please log in or sign up to continue.</p>

        <div className="login-register-container">
          <div className="button-group">
            <button onClick={() => switchLR(true)} className={isLogin ? "active-btn" : ""}>
              Log In
            </button>
            <button onClick={() => switchLR(false)} className={isRegister ? "active-btn" : ""}>
              Sign Up
            </button>
          </div>

          {error && <p className="form-error">{error}</p>}

          {isLogin && (
            <div className="login">
              <form onSubmit={handleLogin}>
                <input
                  type="email"
                  placeholder="Email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                /><br /><br />
                <input
                  type="password"
                  placeholder="Password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                /><br /><br />
                <button type="submit">Submit</button>
              </form>
            </div>
          )}

          {isRegister && (
            <div className="register">
              <form onSubmit={handleRegister}>
                <input
                  type="text"
                  placeholder="Username"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                /><br /><br />
                <input
                  type="email"
                  placeholder="Email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                /><br /><br />
                <input
                  type="password"
                  placeholder="Password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                /><br /><br />
                <input
                  type="password"
                  placeholder="Confirm Password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                /><br /><br />
                <button type="submit">Submit</button>
              </form>
            </div>
          )}
        </div>
      </div>

      <div className="right">
        <img
          src="https://www.amitree.com/wp-content/uploads/2021/08/the-pros-and-cons-of-paper-to-do-lists.jpeg"
          alt="To-Do Illustration"
        />
      </div>
    </div>
  );
};

export default Login;
