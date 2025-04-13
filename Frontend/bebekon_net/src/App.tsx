// App.tsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Header } from "./components/Header/Header";
import { Login } from "./components/Auth/Login/Login";
import { Register } from "./components/Auth/Register/Register";
import {HomePage} from "./components/HomePage/HomePage.tsx";
import {ProfilePage} from "./components/ProfilePage/ProfilePage.tsx";
import {AdminPanel} from "./components/AdminPanel/AdminPanel.tsx";

function App() {
    return (
        <BrowserRouter>
            <Header />
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/adminpanel" element={<AdminPanel />} />
            </Routes>
            <HomePage/>
        </BrowserRouter>
    );
}

export default App;