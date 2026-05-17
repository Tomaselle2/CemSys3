import {
    Handle,
    Position
} from "@xyflow/react";

import {
    useEffect,
    useRef,
    useState
} from "react";

export default function PersonaNode({ data, id }) {

    const [texto, setTexto] = useState(data.label || "");

    const textareaRef = useRef(null);

    useEffect(() => {

        setTexto(data.label || "");

    }, [data.label]);

    useEffect(() => {

        autoResize();

    }, [texto]);

    const autoResize = () => {

        if (!textareaRef.current)
            return;

        textareaRef.current.style.height = 'auto';

        textareaRef.current.style.height =
            textareaRef.current.scrollHeight + 'px';
    };

    const guardarCambio = () => {

        data.onChange(id, texto);
    };

    return (

        <div
            style={{
                position: 'relative'
            }}
        >

            <Handle
                type="target"
                position={Position.Top}
                style={{
                    width: 10,
                    height: 10
                }}
            />

            <div
                style={{
                    background: 'white',
                    border: '1px solid #999',
                    borderRadius: 12,
                    padding: 12,
                    minWidth: 240,
                    boxShadow: '0 2px 8px rgba(0,0,0,0.08)'
                }}
            >

                <div
                    style={{
                        display: 'flex',
                        gap: 10,
                        alignItems: 'flex-start'
                    }}
                >

                    <div
                        style={{
                            fontSize: 28
                        }}
                    >
                        👤
                    </div>

                    <textarea
                        ref={textareaRef}
                        value={texto}
                        onChange={(e) => setTexto(e.target.value)}
                        onBlur={guardarCambio}
                        rows={1}
                        style={{
                            width: '100%',
                            resize: 'none',
                            overflow: 'hidden',
                            border: 'none',
                            outline: 'none',
                            background: 'transparent',
                            fontSize: 18,
                            fontFamily: 'inherit'
                        }}
                    />

                </div>

            </div>

            <Handle
                type="source"
                position={Position.Bottom}
                style={{
                    width: 10,
                    height: 10
                }}
            />

        </div>
    );
}