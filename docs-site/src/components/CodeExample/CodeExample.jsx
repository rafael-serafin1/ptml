import { CopyButton } from "../Button/CopyButton.jsx";

export function CodeExample({ example, index }) {
  return (
    <figure className="example-card">
      <figcaption className="example-heading">
        <span><span className="example-number">{String(index + 1).padStart(2, "0")}</span>{example.title}</span>
        <CopyButton code={example.code} />
      </figcaption>
      <pre className="code-frame"><code>{example.code}</code></pre>
    </figure>
  );
}