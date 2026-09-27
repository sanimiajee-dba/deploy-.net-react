import React, { useState } from "react";

// Calls go through nginx's /api/ proxy to the .NET container - see nginx.conf
const API_BASE = "/api";

export default function App() {
  const [health, setHealth] = useState(null);
  const [dbCheck, setDbCheck] = useState(null);
  const [items, setItems] = useState([]);
  const [error, setError] = useState(null);

  async function callApi(path, setter) {
    setError(null);
    try {
      const res = await fetch(`${API_BASE}${path}`);
      const data = await res.json();
      if (!res.ok) throw new Error(data.detail || "Request failed");
      setter(data);
    } catch (e) {
      setError(e.message);
    }
  }

  return (
    <div style={{ fontFamily: "sans-serif", maxWidth: 600, margin: "40px auto" }}>
      <h1>.NET + React + Oracle 19c Demo</h1>
      <p>Server: 192.168.0.231 — Port: 8088</p>

      <button onClick={() => callApi("/health", setHealth)}>Check API Health</button>
      {health && <pre>{JSON.stringify(health, null, 2)}</pre>}

      <button onClick={() => callApi("/db-check", setDbCheck)}>Check Oracle Connection</button>
      {dbCheck && <pre>{JSON.stringify(dbCheck, null, 2)}</pre>}

      <button onClick={() => callApi("/items", setItems)}>Load Items from Oracle</button>
      <ul>
        {items.map((i) => (
          <li key={i.id}>{i.name}</li>
        ))}
      </ul>

      {error && <p style={{ color: "red" }}>Error: {error}</p>}
    </div>
  );
}
