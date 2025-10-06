import React from 'react';
import { Link } from "react-router-dom";
import '../css/Navbar.css'; // Optional: Add a CSS file for styling
import ReactLogo from '../assets/react.svg';

const Navbar = ({setIsAuthenticated}) => {
    const handleLogout = () => {
        localStorage.removeItem('token');
        setIsAuthenticated(false);
    };

    return (
        <nav className="navbar">
            <div className="navbar-logo">
                <img src={ReactLogo} alt=""></img>
                <Link href="/home">Home</Link>
            </div>
            <ul className="navbar-links">
                <li><Link onClick={handleLogout} className="logout-button">Log Out</Link></li>
            </ul>
        </nav>
    );
};

export default Navbar;