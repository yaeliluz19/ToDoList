
import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import HomePage from './HomePage'; // ייבוא רכיב דף הבית
import Register from './Register';
import Login from './Login';

function App() {
  return (
    <Router>
      <div>
        <nav>
          <Link to="/">Home</Link> | 
          <Link to="/login">Login</Link> | 
          <Link to="/signup">Sign Up</Link>
        </nav>

        <Routes>
          <Route path="/" element={<HomePage />} /> 
          <Route path="/login" element={<Login />} /> 
          <Route path="/signup" element={<Register />} /> 
        </Routes>
      </div>
    </Router>
  );
}

export default App;
