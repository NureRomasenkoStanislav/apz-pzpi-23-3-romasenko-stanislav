import React, { useState } from "react";
import ua from "/src/locales/ua.json";
import en from "/src/locales/en.json";
import { apiService } from "./apiService";
import UserCabinet from "./components/UserCabinet";
import AdminPanel from "./components/AdminPanel";
import { Globe, LogOut } from "lucide-react";

const translations = { ua, en };

export default function App() {
  const [lang, setLang] = useState("ua");
  const [token, setToken] = useState(null);
  const [isAdmin, setIsAdmin] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const t = (key) => {
    const keys = key.split(".");
    return translations[lang][keys[0]]?.[keys[1]] || key;
  };

  const handleLogin = (adminRole) => {
    if (!email || !password) return;
    apiService.login(email, password)
      .then(res => {
        setToken(res.token);
        setIsAdmin(adminRole);
      })
      .catch(() => alert(t("auth.error")));
  };

  const handleLogout = () => {
    setToken(null);
    setIsAdmin(false);
    setEmail("");
    setPassword("");
  };

  return (
    <div>
      {/* Навігаційна панель */}
      <nav style={{ background: "#1e293b", color: "#fff", padding: "14px 24px", display: "flex", justifyContent: "space-between", alignItems: "center", boxShadow: "0 2px 4px rgba(0,0,0,0.05)" }}>
        <h3 style={{ margin: 0, fontWeight: 700, letterSpacing: "0.5px" }}>RoomBook Web</h3>
        <div style={{ display: "flex", alignItems: "center", gap: "20px" }}>
          <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
            <Globe size={18} color="#94a3b8" />
            <select value={lang} onChange={(e) => setLang(e.target.value)} style={{ padding: "6px 12px", margin: 0, background: "#334155", color: "#fff", border: "none" }}>
              <option value="ua">Українська</option>
              <option value="en">English</option>
            </select>
          </div>
          {token && (
            <button onClick={handleLogout} style={{ background: "#ef4444", color: "#fff", width: "auto", padding: "8px 16px", margin: 0, display: "flex", alignItems: "center", gap: "6px" }}>
              <LogOut size={16} /> {t("nav.logout")}
            </button>
          )}
        </div>
      </nav>

      {/* Основний контент */}
      {!token ? (
        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "80vh" }}>
          <div className="card-shadow" style={{ width: "100%", maxWidth: "400px" }}>
            <h3 style={{ textAlign: "center", marginBottom: "24px", marginTop: 0, fontSize: "22px", fontWeight: 700 }}>{t("auth.title")}</h3>
            
            <label style={{ fontSize: "14px", fontWeight: "600", color: "#4b5563" }}>{t("auth.email")}</label>
            <input type="email" placeholder="example@roombook.com" value={email} onChange={e => setEmail(e.target.value)} />
            
            <label style={{ fontSize: "14px", fontWeight: "600", color: "#4b5563" }}>{t("auth.password")}</label>
            <input type="password" placeholder="••••••••" value={password} onChange={e => setPassword(e.target.value)} />
            
            <button onClick={() => handleLogin(false)} style={{ background: "#2563eb", color: "#fff", marginTop: "10px" }}>
              {t("auth.loginUser")}
            </button>
            <button onClick={() => handleLogin(true)} style={{ background: "#7c3aed", color: "#fff" }}>
              {t("auth.loginAdmin")}
            </button>
          </div>
        </div>
      ) : (
        <div>
          <div style={{ background: "#ffffff", padding: "12px 24px", borderBottom: "1px solid #e5e7eb", color: "#4b5563", fontSize: "14px" }}>
            Поточна сесія: <strong>{isAdmin ? t("nav.adminPanel") : t("nav.userCabinet")}</strong>
          </div>
          <div style={{ maxWidth: "1200px", margin: "0 auto", padding: "20px" }}>
            {isAdmin ? <AdminPanel token={token} t={t} /> : <UserCabinet token={token} t={t} lang={lang} />}
          </div>
        </div>
      )}
    </div>
  );
}