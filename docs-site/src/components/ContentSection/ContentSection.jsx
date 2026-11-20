import { Link } from "react-router-dom";
import { CodeExample } from "../CodeExample/CodeExample.jsx";

export function ContentSection({ section, index }) {
  return (
    <section className="content-section" id={`secao-${index + 1}`}>
      <h2>{section.title}</h2>
      <p>{section.text}</p>
      {section.examples?.map((example, exampleIndex) => (
        <CodeExample example={example} index={exampleIndex} key={example.title} />
      ))}
      {section.links && (
        <div className="related-links">
          {section.links.map((item) => (
            <Link className="related-link" key={item.path} to={item.path}>
              {item.title}<span aria-hidden="true">→</span>
            </Link>
          ))}
        </div>
      )}
    </section>
  );
}
