// App.tsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Header } from "./components/Header/Header";
import { Login } from "./components/Auth/Login/Login";
import { Register } from "./components/Auth/Register/Register";

function App() {
    return (
        <BrowserRouter>
            <Header />
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;