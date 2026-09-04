const fs = require('fs');
const path = require('path');

const DB_FILE = path.join(__dirname, 'db.json');

const initialData = {
  supervisors: [
    {
      id: 1,
      fullName: "Prof. Elena Rostova",
      title: "Prof.",
      email: "elena.rostova@university.edu",
      department: "Artificial Intelligence & Robotics",
      researchInterests: "Deep Learning, Medical Computer Vision, Explainable AI",
      officeLocation: "Turing Hall 401",
      maxProjectsQuota: 4,
      avatarUrl: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
      phone: "+1 (555) 234-8901"
    },
    {
      id: 2,
      fullName: "Dr. Marcus Vance",
      title: "Dr.",
      email: "marcus.vance@university.edu",
      department: "Cybersecurity & Information Assurance",
      researchInterests: "Zero-Trust Architecture, Protocol Verification, Cryptanalysis",
      officeLocation: "Cyber Ops Tower 210",
      maxProjectsQuota: 5,
      avatarUrl: "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&auto=format&fit=crop&q=80",
      phone: "+1 (555) 345-9012"
    },
    {
      id: 3,
      fullName: "Dr. Amina Al-Mansoor",
      title: "Dr.",
      email: "amina.almansoor@university.edu",
      department: "Software Engineering & Distributed Systems",
      researchInterests: "Microservices, Event-Driven Architectures, Cloud Native Platforms",
      officeLocation: "Innovation Center 115",
      maxProjectsQuota: 4,
      avatarUrl: "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&auto=format&fit=crop&q=80",
      phone: "+1 (555) 456-0123"
    },
    {
      id: 4,
      fullName: "Assoc. Prof. Kaelen Chen",
      title: "Assoc. Prof.",
      email: "kaelen.chen@university.edu",
      department: "Embedded Systems & Autonomous Hardware",
      researchInterests: "Edge Computing, RTOS, Autonomous Drone Navigation, Sensor Fusion",
      officeLocation: "Hardware Lab 04",
      maxProjectsQuota: 3,
      avatarUrl: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80",
      phone: "+1 (555) 567-1234"
    },
    {
      id: 5,
      fullName: "Dr. David O'Connor",
      title: "Dr.",
      email: "david.oconnor@university.edu",
      department: "Data Science & Quantum Computing",
      researchInterests: "High-Performance Analytics, Graph Neural Networks, LLMs",
      officeLocation: "Quantum Wing 308",
      maxProjectsQuota: 5,
      avatarUrl: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
      phone: "+1 (555) 678-2345"
    }
  ],
  projects: [
    {
      id: 1,
      title: "NeuroScan: Multimodal AI for Early Neurological Anomaly Detection",
      abstract: "A clinical decision support platform leveraging 3D Transformer models and multimodal MRI/PET fusion to identify early-stage neurodegenerative biomarkers with 94.8% accuracy. Includes an interactive 3D volumetric viewer, uncertainty estimation heatmaps, and FHIR interoperability for clinical PACS integration.",
      description: "NeuroScan addresses the latency in diagnostic pipelines by accelerating volumetric MRI analysis from hours to under 45 seconds. Built with PyTorch, ONNX Runtime, ASP.NET Core API, and WebGL DICOM rendering engine.",
      track: "Artificial Intelligence",
      status: "Defended",
      academicYear: "2025-2026",
      semester: "Fall",
      supervisorId: 1,
      finalGrade: 96.5,
      techStack: "PyTorch, ASP.NET Core, React, Three.js, ONNX, PostgreSQL, Docker",
      repositoryUrl: "https://github.com/grad-projects/neuroscan-ai",
      demoUrl: "https://neuroscan-demo.internal.university.edu",
      documentationUrl: "https://docs.neuroscan.internal/final-report.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1559757175-5700dde675bc?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-08-20T10:00:00Z",
      defenseRoom: "Auditorium A-101",
      createdAt: "2026-03-01T08:00:00Z",
      teamMembers: [
        { id: 1, fullName: "Tariq Mansour", studentId: "ST-2022-8401", email: "tariq.mansour@university.edu", role: "Team Lead & ML Architect", major: "AI & Data Science", avatarUrl: "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?w=100&auto=format&fit=crop&q=80" },
        { id: 2, fullName: "Layla Chen", studentId: "ST-2022-8402", email: "layla.chen@university.edu", role: "Full Stack Engineer", major: "Software Engineering", avatarUrl: "https://images.unsplash.com/photo-1517841905240-472988babdf9?w=100&auto=format&fit=crop&q=80" },
        { id: 3, fullName: "Zaid Al-Harbi", studentId: "ST-2022-8403", email: "zaid.alharbi@university.edu", role: "Systems & Cloud Engineer", major: "Computer Engineering", avatarUrl: "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 101, title: "Literature Review & Model Architecture", dueDate: "2026-04-15", status: "Approved", weightPercentage: 20, feedback: "Excellent benchmark methodology." },
        { id: 102, title: "Model Training & Multimodal Fusion", dueDate: "2026-06-01", status: "Approved", weightPercentage: 30, feedback: "Exceptional results with cross-attention fusion." },
        { id: 103, title: "Clinical Web PACS Integration", dueDate: "2026-07-20", status: "Approved", weightPercentage: 30, feedback: "Clean architecture and DICOM standard conformance." },
        { id: 104, title: "Final Thesis & Defense Presentation", dueDate: "2026-08-20", status: "Approved", weightPercentage: 20, feedback: "Distinguished capstone contribution." }
      ],
      evaluations: [
        { id: 1, evaluatorName: "Prof. Elena Rostova", evaluatorRole: "Primary Supervisor", presentationScore: 19.5, implementationScore: 39.0, documentationScore: 19.0, innovationScore: 19.0, totalScore: 96.5, comments: "Groundbreaking research contribution with tangible healthcare impact. Code quality and testing coverage are exemplary.", evaluationDate: "2026-08-20T11:30:00Z" },
        { id: 2, evaluatorName: "Dr. Marcus Vance", evaluatorRole: "Internal Committee Chair", presentationScore: 19.0, implementationScore: 38.5, documentationScore: 19.5, innovationScore: 19.5, totalScore: 96.5, comments: "Outstanding defense. Clear mastery of both machine learning mathematics and software architecture.", evaluationDate: "2026-08-20T11:45:00Z" }
      ]
    },
    {
      id: 2,
      title: "AegisGate: Decentralized Zero-Trust Micro-Segmentation for Industrial SCADA",
      abstract: "A kernel-level eBPF packet inspection framework with cryptographic hardware attestation (TPM 2.0) that isolates vulnerable PLC controllers and automates real-time anomaly quarantine in critical water treatment and energy grids.",
      description: "AegisGate prevents lateral movement in OT environments using behavioral baselining and mutual TLS mesh routing without modifying legacy PLC firmware.",
      track: "Cybersecurity",
      status: "Ready for Defense",
      academicYear: "2025-2026",
      semester: "Spring",
      supervisorId: 2,
      finalGrade: null,
      techStack: "Rust, eBPF, Linux Kernel, C#, Go, WireGuard, Prometheus, Grafana",
      repositoryUrl: "https://github.com/grad-projects/aegis-gate-zt",
      demoUrl: "https://aegisgate-sim.internal.university.edu",
      documentationUrl: "https://docs.aegisgate.internal/whitepaper.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-09-08T14:00:00Z",
      defenseRoom: "Cyber Arena Lab 3B",
      createdAt: "2026-04-10T09:30:00Z",
      teamMembers: [
        { id: 4, fullName: "Farah Qasim", studentId: "ST-2022-7911", email: "farah.qasim@university.edu", role: "Team Lead & Kernel Dev", major: "Cybersecurity", avatarUrl: "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?w=100&auto=format&fit=crop&q=80" },
        { id: 5, fullName: "Dmitri Volkov", studentId: "ST-2022-7912", email: "dmitri.volkov@university.edu", role: "Protocol Engineer", major: "Computer Networks", avatarUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 201, title: "Threat Modeling & eBPF Prototype", dueDate: "2026-05-10", status: "Approved", weightPercentage: 25, feedback: "Very thorough security analysis." },
        { id: 202, title: "Zero-Trust Mesh & TPM 2.0 Attestation", dueDate: "2026-07-05", status: "Approved", weightPercentage: 35, feedback: "Solid cryptographic implementation." },
        { id: 203, title: "Testbed Penetration Testing & Report", dueDate: "2026-09-02", status: "Approved", weightPercentage: 40, feedback: "Ready for final defense committee." }
      ],
      evaluations: []
    },
    {
      id: 3,
      title: "AeroMesh: Autonomous Swarm Coordination for Disaster Search & Rescue",
      abstract: "A distributed ad-hoc mesh networking protocol for aerial drone swarms operating in GPS-denied collapsed structures. Utilizes onboard stereo Visual-Inertial Odometry (VIO) and distributed SLAM mapping to generate 3D hazard maps in real time.",
      description: "Field-tested with a 5-drone quadcopter testbed. Operates over 915MHz LoRa fallback and 5GHz Wi-Fi ad-hoc mesh with zero centralized base station dependency.",
      track: "IoT & Embedded",
      status: "In Progress",
      academicYear: "2025-2026",
      semester: "Spring",
      supervisorId: 4,
      finalGrade: null,
      techStack: "C++, ROS2 Humble, PX4 Autopilot, OpenCV, FreeRTOS, ESP32, Python",
      repositoryUrl: "https://github.com/grad-projects/aeromesh-swarm",
      demoUrl: "https://aeromesh-viz.internal.university.edu",
      documentationUrl: "https://docs.aeromesh.internal/architecture.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1527977966376-1c8408f9f108?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-09-28T11:00:00Z",
      defenseRoom: "Robotics Arena East",
      createdAt: "2026-05-02T10:00:00Z",
      teamMembers: [
        { id: 6, fullName: "Sara Al-Otaibi", studentId: "ST-2022-6320", email: "sara.alotaibi@university.edu", role: "Team Lead & VIO Specialist", major: "Robotics & Embedded", avatarUrl: "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=100&auto=format&fit=crop&q=80" },
        { id: 7, fullName: "Lucas Romero", studentId: "ST-2022-6321", email: "lucas.romero@university.edu", role: "Firmware & Mesh Hardware", major: "Computer Engineering", avatarUrl: "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 301, title: "LoRa/Wi-Fi Ad-Hoc Protocol Stack", dueDate: "2026-06-15", status: "Approved", weightPercentage: 30, feedback: "Good throughput benchmark." },
        { id: 302, title: "Distributed VIO SLAM & Point Cloud", dueDate: "2026-09-12", status: "InProgress", weightPercentage: 40, feedback: "Field calibration underway." },
        { id: 303, title: "Live Obstacle Avoidance Flight Test", dueDate: "2026-09-25", status: "Pending", weightPercentage: 30, feedback: null }
      ],
      evaluations: []
    },
    {
      id: 4,
      title: "KubePulse: Predictive Autoscaler & Carbon-Aware Workload Orchestrator",
      abstract: "An intelligent Kubernetes custom controller that integrates real-time grid carbon emission APIs with temporal convolution networks (TCN) to dynamically migrate non-urgent batch computations to regions with lowest carbon intensity while preserving strict SLA constraints.",
      description: "Evaluated on a 48-node hybrid cluster, achieving a 31.4% carbon footprint reduction and 18% cloud spend optimization compared to default Horizontal Pod Autoscalers.",
      track: "Cloud & DevOps",
      status: "Defended",
      academicYear: "2025-2026",
      semester: "Fall",
      supervisorId: 3,
      finalGrade: 94.0,
      techStack: "Go, Kubernetes Operator SDK, C#, TimescaleDB, Grafana, gRPC, Terraform",
      repositoryUrl: "https://github.com/grad-projects/kubepulse-controller",
      demoUrl: "https://kubepulse.internal.university.edu",
      documentationUrl: "https://docs.kubepulse.internal/thesis.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1618401471353-b98aedd04e11?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-08-15T15:00:00Z",
      defenseRoom: "Engineering Room 204",
      createdAt: "2026-02-15T14:00:00Z",
      teamMembers: [
        { id: 8, fullName: "Yasmine Benali", studentId: "ST-2022-5501", email: "yasmine.benali@university.edu", role: "Team Lead & K8s Dev", major: "Cloud Computing", avatarUrl: "https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=100&auto=format&fit=crop&q=80" },
        { id: 9, fullName: "Arjun Mehta", studentId: "ST-2022-5502", email: "arjun.mehta@university.edu", role: "Backend & Metrics Engineer", major: "Software Engineering", avatarUrl: "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 401, title: "Carbon Emission Model Integration", dueDate: "2026-04-10", status: "Approved", weightPercentage: 25, feedback: "Solid integration with national grid telemetry." },
        { id: 402, title: "Controller Scheduler Implementation", dueDate: "2026-06-20", status: "Approved", weightPercentage: 45, feedback: "Smooth CRD reconciliation loop." },
        { id: 403, title: "Cluster Evaluation Benchmark", dueDate: "2026-08-10", status: "Approved", weightPercentage: 30, feedback: "Verified 31% reduction." }
      ],
      evaluations: [
        { id: 3, evaluatorName: "Dr. Amina Al-Mansoor", evaluatorRole: "Primary Supervisor", presentationScore: 18.5, implementationScore: 38.0, documentationScore: 18.5, innovationScore: 19.0, totalScore: 94.0, comments: "Significant reduction in cloud carbon footprint validated on realistic distributed clusters. High quality engineering.", evaluationDate: "2026-08-15T16:00:00Z" }
      ]
    },
    {
      id: 5,
      title: "VeriDoc: Zero-Knowledge Academic Credential & Skill Verification Protocol",
      abstract: "A sovereign verifiable credentials ecosystem enabling educational institutions to cryptographically stamp transcripts and diploma claims using zk-SNARKs, allowing students to prove GPA thresholds or prerequisites to employers without revealing private full transcripts.",
      description: "Compliant with W3C Verifiable Credentials and OpenBadges 3.0 standards. Includes mobile wallet for iOS/Android and enterprise instant verification portal.",
      track: "Software Engineering",
      status: "Approved",
      academicYear: "2025-2026",
      semester: "Spring",
      supervisorId: 2,
      finalGrade: null,
      techStack: "Solidity, Circom zk-SNARKs, ASP.NET Core 8, TypeScript, TailwindCSS, PostgreSQL",
      repositoryUrl: "https://github.com/grad-projects/veridoc-zk",
      demoUrl: "https://veridoc-app.internal.university.edu",
      documentationUrl: "https://docs.veridoc.internal/spec.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-10-10T13:00:00Z",
      defenseRoom: "Turing Hall 302",
      createdAt: "2026-06-01T11:00:00Z",
      teamMembers: [
        { id: 10, fullName: "Maya Haddad", studentId: "ST-2022-4110", email: "maya.haddad@university.edu", role: "Lead Protocol Architect", major: "Software Engineering", avatarUrl: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 501, title: "zk-SNARK Circuit Specification", dueDate: "2026-07-15", status: "Approved", weightPercentage: 35, feedback: "Clean zero-knowledge constraints." },
        { id: 502, title: "W3C Verifiable Credential Wallet", dueDate: "2026-09-15", status: "InProgress", weightPercentage: 40, feedback: "Testing key exchange." },
        { id: 503, title: "Employer Verification Portal", dueDate: "2026-10-01", status: "Pending", weightPercentage: 25, feedback: null }
      ],
      evaluations: []
    },
    {
      id: 6,
      title: "BioGraph: Graph Neural Networks for Drug-Target Interaction Screening",
      abstract: "A computational pharmacology platform constructing heterogeneous biomedical knowledge graphs from PubMed literature, ChEMBL chemical structures, and UniProt protein databases to predict novel compound binding affinities for rare oncological targets.",
      description: "Employs Relational Graph Attention Networks (R-GAT) with physics-informed molecular graph embeddings to accelerate in-silico discovery pipeline.",
      track: "Data Science",
      status: "In Progress",
      academicYear: "2025-2026",
      semester: "Spring",
      supervisorId: 5,
      finalGrade: null,
      techStack: "PyTorch Geometric, Neo4j, Python FastAPI, .NET WebApp, D3.js, BioPython",
      repositoryUrl: "https://github.com/grad-projects/biograph-gnn",
      demoUrl: "https://biograph.internal.university.edu",
      documentationUrl: "https://docs.biograph.internal/report.pdf",
      thumbnailUrl: "https://images.unsplash.com/photo-1532187863486-abf9dbad1b69?w=600&auto=format&fit=crop&q=80",
      defenseDate: "2026-09-22T14:30:00Z",
      defenseRoom: "Life Sciences 104",
      createdAt: "2026-05-15T09:00:00Z",
      teamMembers: [
        { id: 11, fullName: "Nikhil Sharma", studentId: "ST-2022-3890", email: "nikhil.sharma@university.edu", role: "GNN Researcher", major: "Data Science", avatarUrl: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100&auto=format&fit=crop&q=80" }
      ],
      milestones: [
        { id: 601, title: "Knowledge Graph Ingestion Pipeline", dueDate: "2026-06-30", status: "Approved", weightPercentage: 30, feedback: "1.2M nodes ingested successfully." },
        { id: 602, title: "R-GAT Architecture & Training", dueDate: "2026-09-10", status: "InProgress", weightPercentage: 45, feedback: "Achieving 0.89 ROC-AUC." },
        { id: 603, title: "Target Binding Validation", dueDate: "2026-09-20", status: "Pending", weightPercentage: 25, feedback: null }
      ],
      evaluations: []
    }
  ],
  announcements: [
    {
      id: 1,
      title: "Spring 2026 Final Defense Schedule & Committee Allocations Released",
      content: "The committee defense timetable for all graduating seniors is now officially scheduled. Please verify your assigned defense hall, time slot, and bring 3 bound copies of your project report.",
      category: "Defense",
      priority: "Urgent",
      authorName: "Graduation Project Committee",
      publishedDate: "2026-09-02T08:00:00Z",
      actionLabel: "View Defense Schedule",
      actionUrl: "/defense"
    },
    {
      id: 2,
      title: "Mandatory Plagiarism & Code Originality Verification (Turnitin/Moss)",
      content: "All project teams must submit their final thesis documents to the Turnitin portal and push final git repositories before the cutoff. Similarity index must not exceed 15% excluding standard library references.",
      category: "Guidelines",
      priority: "Important",
      authorName: "Dean of Academic Quality",
      publishedDate: "2026-08-30T10:00:00Z",
      actionLabel: "Read Submission Policy",
      actionUrl: "/projects"
    },
    {
      id: 3,
      title: "Annual University Innovation & Tech Showcase Registration Open",
      content: "Top-tier graduation projects will be selected for presentation at the Annual Engineering Expo in front of industry partners and venture scouts. High-impact awards and funding grants available.",
      category: "General",
      priority: "Normal",
      authorName: "Industry Relations Office",
      publishedDate: "2026-08-25T14:00:00Z",
      actionLabel: "Submit Project for Expo",
      actionUrl: "/projects#submit"
    }
  ]
};

function readDb() {
  if (!fs.existsSync(DB_FILE)) {
    fs.mkdirSync(path.dirname(DB_FILE), { recursive: true });
    fs.writeFileSync(DB_FILE, JSON.stringify(initialData, null, 2));
    return JSON.parse(JSON.stringify(initialData));
  }
  try {
    const raw = fs.readFileSync(DB_FILE, 'utf8');
    return JSON.parse(raw);
  } catch (e) {
    return JSON.parse(JSON.stringify(initialData));
  }
}

function writeDb(data) {
  fs.mkdirSync(path.dirname(DB_FILE), { recursive: true });
  fs.writeFileSync(DB_FILE, JSON.stringify(data, null, 2));
}

module.exports = {
  getSupervisors: () => {
    const db = readDb();
    return db.supervisors.map(s => {
      const supProjects = db.projects.filter(p => p.supervisorId === s.id);
      return {
        ...s,
        currentProjectsCount: supProjects.length,
        availableCapacity: Math.max(0, s.maxProjectsQuota - supProjects.length),
        supervisedProjects: supProjects.map(p => ({ id: p.id, title: p.title, status: p.status, track: p.track }))
      };
    });
  },

  getProjects: (filters = {}) => {
    const db = readDb();
    let result = db.projects.map(p => {
      const supervisor = db.supervisors.find(s => s.id === p.supervisorId) || null;
      return { ...p, supervisor };
    });

    if (filters.search) {
      const s = filters.search.trim().toLowerCase();
      result = result.filter(p => 
        p.title.toLowerCase().includes(s) ||
        p.abstract.toLowerCase().includes(s) ||
        (p.techStack && p.techStack.toLowerCase().includes(s)) ||
        (p.supervisor && p.supervisor.fullName.toLowerCase().includes(s)) ||
        p.teamMembers.some(m => m.fullName.toLowerCase().includes(s))
      );
    }

    if (filters.track) {
      result = result.filter(p => p.track.toLowerCase() === filters.track.toLowerCase());
    }

    if (filters.status) {
      result = result.filter(p => p.status.toLowerCase() === filters.status.toLowerCase());
    }

    return result;
  },

  getProjectById: (id) => {
    const db = readDb();
    const p = db.projects.find(x => x.id === parseInt(id));
    if (!p) return null;
    const supervisor = db.supervisors.find(s => s.id === p.supervisorId) || null;
    return { ...p, supervisor };
  },

  addProject: (dto) => {
    const db = readDb();
    const newId = db.projects.length ? Math.max(...db.projects.map(p => p.id)) + 1 : 1;
    const project = {
      id: newId,
      title: dto.title,
      abstract: dto.abstract,
      description: dto.description || dto.abstract,
      track: dto.track || "Software Engineering",
      status: "Proposed",
      academicYear: "2025-2026",
      semester: "Spring",
      supervisorId: dto.supervisorId ? parseInt(dto.supervisorId) : null,
      finalGrade: null,
      techStack: dto.techStack || "",
      repositoryUrl: dto.repositoryUrl || null,
      demoUrl: dto.demoUrl || null,
      documentationUrl: dto.documentationUrl || null,
      thumbnailUrl: dto.thumbnailUrl || "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=600&auto=format&fit=crop&q=80",
      defenseDate: null,
      defenseRoom: null,
      createdAt: new Date().toISOString(),
      teamMembers: dto.teamMembers || [],
      milestones: [
        { id: newId * 100 + 1, title: "Project Proposal & Requirements", dueDate: new Date(Date.now() + 14 * 86400000).toISOString().split('T')[0], status: "Pending", weightPercentage: 15, feedback: null },
        { id: newId * 100 + 2, title: "Architecture & Prototype", dueDate: new Date(Date.now() + 45 * 86400000).toISOString().split('T')[0], status: "Pending", weightPercentage: 25, feedback: null },
        { id: newId * 100 + 3, title: "Full Implementation & Testing", dueDate: new Date(Date.now() + 90 * 86400000).toISOString().split('T')[0], status: "Pending", weightPercentage: 40, feedback: null },
        { id: newId * 100 + 4, title: "Final Defense & Report Submission", dueDate: new Date(Date.now() + 120 * 86400000).toISOString().split('T')[0], status: "Pending", weightPercentage: 20, feedback: null }
      ],
      evaluations: []
    };

    db.projects.unshift(project);
    writeDb(db);
    return project;
  },

  addEvaluation: (dto) => {
    const db = readDb();
    const project = db.projects.find(p => p.id === parseInt(dto.projectId));
    if (!project) return null;

    const evalId = project.evaluations.length ? Math.max(...project.evaluations.map(e => e.id)) + 1 : 1;
    const presentation = Math.min(20, Math.max(0, parseFloat(dto.presentationScore) || 0));
    const implementation = Math.min(40, Math.max(0, parseFloat(dto.implementationScore) || 0));
    const documentation = Math.min(20, Math.max(0, parseFloat(dto.documentationScore) || 0));
    const innovation = Math.min(20, Math.max(0, parseFloat(dto.innovationScore) || 0));
    const totalScore = parseFloat((presentation + implementation + documentation + innovation).toFixed(1));

    const evalObj = {
      id: evalId,
      evaluatorName: dto.evaluatorName || "Committee Examiner",
      evaluatorRole: dto.evaluatorRole || "Internal Examiner",
      presentationScore: presentation,
      implementationScore: implementation,
      documentationScore: documentation,
      innovationScore: innovation,
      totalScore: totalScore,
      comments: dto.comments || "",
      evaluationDate: new Date().toISOString()
    };

    project.evaluations.push(evalObj);
    const sum = project.evaluations.reduce((acc, e) => acc + e.totalScore, 0);
    project.finalGrade = parseFloat((sum / project.evaluations.length).toFixed(1));
    if (project.status !== "Defended") {
      project.status = "Defended";
    }

    writeDb(db);
    return { evalObj, projectFinalGrade: project.finalGrade };
  },

  updateMilestoneStatus: (projectId, milestoneId, newStatus) => {
    const db = readDb();
    const project = db.projects.find(p => p.id === parseInt(projectId));
    if (!project) return null;
    const ms = project.milestones.find(m => m.id === parseInt(milestoneId));
    if (!ms) return null;
    ms.status = newStatus;
    writeDb(db);
    return ms;
  },

  getAnnouncements: () => {
    const db = readDb();
    return db.announcements;
  },

  getStats: () => {
    const db = readDb();
    const totalProjects = db.projects.length;
    const defendedProjects = db.projects.filter(p => p.status === 'Defended').length;
    const inProgressProjects = db.projects.filter(p => p.status === 'In Progress' || p.status === 'Ready for Defense' || p.status === 'Approved').length;
    const totalStudents = db.projects.reduce((acc, p) => acc + p.teamMembers.length, 0);
    const totalSupervisors = db.supervisors.length;
    
    const graded = db.projects.filter(p => p.finalGrade !== null);
    const avgScore = graded.length ? parseFloat((graded.reduce((acc, p) => acc + p.finalGrade, 0) / graded.length).toFixed(1)) : 0;

    const upcomingDefenses = db.projects
      .filter(p => p.defenseDate && new Date(p.defenseDate) >= new Date())
      .map(p => ({
        id: p.id,
        title: p.title,
        defenseDate: p.defenseDate,
        defenseRoom: p.defenseRoom,
        supervisor: db.supervisors.find(s => s.id === p.supervisorId)?.fullName || "TBD"
      }));

    return {
      totalProjects,
      defendedProjects,
      inProgressProjects,
      totalStudents,
      totalSupervisors,
      avgScore,
      upcomingDefenses
    };
  }
};