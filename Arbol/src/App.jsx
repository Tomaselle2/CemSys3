import { useEffect, useCallback } from "react";
import PersonaNode from "./PersonaNode";

import {
    ReactFlow,
    Background,
    Controls,
    addEdge,
    useNodesState,
    useEdgesState,
    ConnectionLineType
} from "@xyflow/react";

import "@xyflow/react/dist/style.css";

export default function App() {
    const nodeTypes = {
        persona: PersonaNode
    };

    const root = document.getElementById("react-flow-root");

    const tramiteId = root.dataset.tramiteId;

    const [nodes, setNodes, onNodesChange] = useNodesState([]);

    const [edges, setEdges, onEdgesChange] = useEdgesState([]);

    const onConnect = useCallback(
        (params) => setEdges((eds) => addEdge(params, eds)),
        []
    );

    const agregarPersona = () => {

        const nuevoNodo = {

            id: crypto.randomUUID(),

            type: 'persona',

            position: {
                x: 250,
                y: 250
            },

            data: {
                label: '',
                onChange: actualizarTextoNodo
            }
        };

        setNodes((nds) => [...nds, nuevoNodo]);
    };

    const actualizarTextoNodo = (id, value) => {

        setNodes((nds) =>
            nds.map((node) => {

                if (node.id === id) {

                    node.data = {
                        ...node.data,
                        label: value
                    };
                }

                return node;
            })
        );
    };

    useEffect(() => {

        fetch(`/Arbol/ObtenerDiagrama?tramiteId=${tramiteId}`)
            .then(r => r.json())
            .then(data => {

                if (data.jsonDiagrama) {

                    const flow = JSON.parse(data.jsonDiagrama);

                    const nodesConEventos = (flow.nodes || []).map(node => ({
                        ...node,
                        data: {
                            ...node.data,
                            onChange: actualizarTextoNodo
                        }
                    }));

                    setNodes(nodesConEventos);
                    setEdges(flow.edges || []);
                }
            });

    }, []);

    const guardar = async () => {

        const flow = {

            nodes: nodes.map(n => ({
                ...n,
                data: {
                    ...n.data,
                    onChange: undefined
                }
            })),

            edges
        };

        await fetch('/Arbol/GuardarDiagrama', {

            method: 'POST',

            headers: {
                'Content-Type': 'application/json'
            },

            body: JSON.stringify({

                tramiteId,
                jsonDiagrama: JSON.stringify(flow)
            })
        });

        AlertService.show("Éxito", "Guardado correctamente", "success");
    };


    return (
        <div style={{ width: '100%', height: '700px' }}>

            <button className="btn btn-primary m-2" onClick={agregarPersona}>
                 👤 Persona
            </button>

            <button className="btn btn-success m-2" onClick={guardar}>
                Guardar
            </button>

            <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                onNodesChange={onNodesChange}
                onEdgesChange={onEdgesChange}
                onConnect={onConnect}

                deleteKeyCode={["Delete", "Backspace"]}

                
                connectionLineType="smoothstep"

                defaultEdgeOptions={{
                    type: 'smoothstep',
                    style: {
                        strokeWidth: 2
                    }
                }}
                fitView
            >
                <Background />
                <Controls />
            </ReactFlow>

        </div>
    );
}