using System;
using System.Collections.Generic;
using System.Linq;
using GraduationProject.Models;

namespace GraduationProject.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Projects.Any())
            {
                return; // Database has been seeded
            }

            // 1. Supervisors
            var supervisors = new List<Supervisor>
            {
                new Supervisor
                {
                    FullName = "Elena Rostova",
                    Title = "Prof.",
                    Email = "elena.rostova@university.edu",
                    Department = "Artificial Intelligence & Robotics",
                    ResearchInterests = "Deep Learning, Medical Computer Vision, Explainable AI",
                    OfficeLocation = "Turing Hall 401",
                    MaxProjectsQuota = 4,
                    AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
                    Phone = "+1 (555) 234-8901"
                },
                new Supervisor
                {
                    FullName = "Marcus Vance",
                    Title = "Dr.",
                    Email = "marcus.vance@university.edu",
                    Department = "Cybersecurity & Information Assurance",
                    ResearchInterests = "Zero-Trust Architecture, Protocol Verification, Cryptanalysis",
                    OfficeLocation = "Cyber Ops Tower 210",
                    MaxProjectsQuota = 5,
                    AvatarUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&auto=format&fit=crop&q=80",
                    Phone = "+1 (555) 345-9012"
                },
                new Supervisor
                {
                    FullName = "Amina Al-Mansoor",
                    Title = "Dr.",
                    Email = "amina.almansoor@university.edu",
                    Department = "Software Engineering & Distributed Systems",
                    ResearchInterests = "Microservices, Event-Driven Architectures, Cloud Native Platforms",
                    OfficeLocation = "Innovation Center 115",
                    MaxProjectsQuota = 4,
                    AvatarUrl = "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&auto=format&fit=crop&q=80",
                    Phone = "+1 (555) 456-0123"
                },
                new Supervisor
                {
                    FullName = "Kaelen Chen",
                    Title = "Assoc. Prof.",
                    Email = "kaelen.chen@university.edu",
                    Department = "Embedded Systems & Autonomous Hardware",
                    ResearchInterests = "Edge Computing, RTOS, Autonomous Drone Navigation, Sensor Fusion",
                    OfficeLocation = "Hardware Lab 04",
                    MaxProjectsQuota = 3,
                    AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80",
                    Phone = "+1 (555) 567-1234"
                },
                new Supervisor
                {
                    FullName = "David O'Connor",
                    Title = "Dr.",
                    Email = "david.oconnor@university.edu",
                    Department = "Data Science & Quantum Computing",
                    ResearchInterests = "High-Performance Analytics, Graph Neural Networks, Large Language Models",
                    OfficeLocation = "Quantum Wing 308",
                    MaxProjectsQuota = 5,
                    AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
                    Phone = "+1 (555) 678-2345"
                }
            };
            context.Supervisors.AddRange(supervisors);
            context.SaveChanges();

            // 2. Projects
            var p1 = new Project
            {
                Title = "NeuroScan: Multimodal AI for Early Neurological Anomaly Detection",
                Abstract = "A clinical decision support platform leveraging 3D Transformer models and multimodal MRI/PET fusion to identify early-stage neurodegenerative biomarkers with 94.8% accuracy. Includes an interactive 3D volumetric viewer, uncertainty estimation heatmaps, and FHIR interoperability for clinical PACS integration.",
                Description = "NeuroScan addresses the latency in diagnostic pipelines by accelerating volumetric MRI analysis from hours to under 45 seconds. Built with PyTorch, ONNX Runtime, ASP.NET Core API, and WebGL DICOM rendering engine.",
                Track = ProjectTrack.ArtificialIntelligence,
                Status = ProjectStatus.Defended,
                AcademicYear = "2025-2026",
                Semester = "Fall",
                SupervisorId = supervisors[0].Id,
                FinalGrade = 96.5,
                TechStack = "PyTorch, ASP.NET Core, React, Three.js, ONNX, PostgreSQL, Docker",
                RepositoryUrl = "https://github.com/grad-projects/neuroscan-ai",
                DemoUrl = "https://neuroscan-demo.internal.university.edu",
                DocumentationUrl = "https://docs.neuroscan.internal/final-report.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1559757175-5700dde675bc?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(-14),
                DefenseRoom = "Auditorium A-101",
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            };

            var p2 = new Project
            {
                Title = "AegisGate: Decentralized Zero-Trust Micro-Segmentation for Industrial SCADA",
                Abstract = "A kernel-level eBPF packet inspection framework with cryptographic hardware attestation (TPM 2.0) that isolates vulnerable PLC controllers and automates real-time anomaly quarantine in critical water treatment and energy grids.",
                Description = "AegisGate prevents lateral movement in OT environments using behavioral baselining and mutual TLS mesh routing without modifying legacy PLC firmware.",
                Track = ProjectTrack.Cybersecurity,
                Status = ProjectStatus.ReadyForDefense,
                AcademicYear = "2025-2026",
                Semester = "Spring",
                SupervisorId = supervisors[1].Id,
                FinalGrade = null,
                TechStack = "Rust, eBPF, Linux Kernel, C#, Go, WireGuard, Prometheus, Grafana",
                RepositoryUrl = "https://github.com/grad-projects/aegis-gate-zt",
                DemoUrl = "https://aegisgate-sim.internal.university.edu",
                DocumentationUrl = "https://docs.aegisgate.internal/whitepaper.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(3),
                DefenseRoom = "Cyber Arena Lab 3B",
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            };

            var p3 = new Project
            {
                Title = "AeroMesh: Autonomous Swarm Coordination for Disaster Search & Rescue",
                Abstract = "A distributed ad-hoc mesh networking protocol for aerial drone swarms operating in GPS-denied collapsed structures. Utilizes onboard stereo Visual-Inertial Odometry (VIO) and distributed SLAM mapping to generate 3D hazard maps in real time.",
                Description = "Field-tested with a 5-drone quadcopter testbed. Operates over 915MHz LoRa fallback and 5GHz Wi-Fi ad-hoc mesh with zero centralized base station dependency.",
                Track = ProjectTrack.IoTAndEmbedded,
                Status = ProjectStatus.InProgress,
                AcademicYear = "2025-2026",
                Semester = "Spring",
                SupervisorId = supervisors[3].Id,
                FinalGrade = null,
                TechStack = "C++, ROS2 Humble, PX4 Autopilot, OpenCV, FreeRTOS, ESP32, Python",
                RepositoryUrl = "https://github.com/grad-projects/aeromesh-swarm",
                DemoUrl = "https://aeromesh-viz.internal.university.edu",
                DocumentationUrl = "https://docs.aeromesh.internal/architecture.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1527977966376-1c8408f9f108?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(25),
                DefenseRoom = "Robotics Arena East",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            };

            var p4 = new Project
            {
                Title = "KubePulse: Predictive Autoscaler & Carbon-Aware Workload Orchestrator",
                Abstract = "An intelligent Kubernetes custom controller that integrates real-time grid carbon emission APIs with temporal convolution networks (TCN) to dynamically migrate non-urgent batch computations to regions with lowest carbon intensity while preserving strict SLA constraints.",
                Description = "Evaluated on a 48-node hybrid cluster, achieving a 31.4% carbon footprint reduction and 18% cloud spend optimization compared to default Horizontal Pod Autoscalers.",
                Track = ProjectTrack.CloudAndDevOps,
                Status = ProjectStatus.Defended,
                AcademicYear = "2025-2026",
                Semester = "Fall",
                SupervisorId = supervisors[2].Id,
                FinalGrade = 94.0,
                TechStack = "Go, Kubernetes Operator SDK, C#, TimescaleDB, Grafana, gRPC, Terraform",
                RepositoryUrl = "https://github.com/grad-projects/kubepulse-controller",
                DemoUrl = "https://kubepulse.internal.university.edu",
                DocumentationUrl = "https://docs.kubepulse.internal/thesis.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1618401471353-b98aedd04e11?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(-20),
                DefenseRoom = "Engineering Room 204",
                CreatedAt = DateTime.UtcNow.AddMonths(-7)
            };

            var p5 = new Project
            {
                Title = "VeriDoc: Zero-Knowledge Academic Credential & Skill Verification Protocol",
                Abstract = "A sovereign verifiable credentials ecosystem enabling educational institutions to cryptographically stamp transcripts and diploma claims using zk-SNARKs, allowing students to prove GPA thresholds or prerequisites to employers without revealing private full transcripts.",
                Description = "Compliant with W3C Verifiable Credentials and OpenBadges 3.0 standards. Includes mobile wallet for iOS/Android and enterprise instant verification portal.",
                Track = ProjectTrack.SoftwareEngineering,
                Status = ProjectStatus.Approved,
                AcademicYear = "2025-2026",
                Semester = "Spring",
                SupervisorId = supervisors[1].Id,
                FinalGrade = null,
                TechStack = "Solidity, Circom zk-SNARKs, ASP.NET Core 8, TypeScript, TailwindCSS, PostgreSQL",
                RepositoryUrl = "https://github.com/grad-projects/veridoc-zk",
                DemoUrl = "https://veridoc-app.internal.university.edu",
                DocumentationUrl = "https://docs.veridoc.internal/spec.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(40),
                DefenseRoom = "Turing Hall 302",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            };

            var p6 = new Project
            {
                Title = "BioGraph: Graph Neural Networks for Drug-Target Interaction Screening",
                Abstract = "A computational pharmacology platform constructing heterogeneous biomedical knowledge graphs from PubMed literature, ChEMBL chemical structures, and UniProt protein databases to predict novel compound binding affinities for rare oncological targets.",
                Description = "Employs Relational Graph Attention Networks (R-GAT) with physics-informed molecular graph embeddings to accelerate in-silico discovery pipeline.",
                Track = ProjectTrack.DataScienceAndAnalytics,
                Status = ProjectStatus.InProgress,
                AcademicYear = "2025-2026",
                Semester = "Spring",
                SupervisorId = supervisors[4].Id,
                FinalGrade = null,
                TechStack = "PyTorch Geometric, Neo4j, Python FastAPI, .NET WebApp, D3.js, BioPython",
                RepositoryUrl = "https://github.com/grad-projects/biograph-gnn",
                DemoUrl = "https://biograph.internal.university.edu",
                DocumentationUrl = "https://docs.biograph.internal/report.pdf",
                ThumbnailUrl = "https://images.unsplash.com/photo-1532187863486-abf9dbad1b69?w=600&auto=format&fit=crop&q=80",
                DefenseDate = DateTime.UtcNow.AddDays(18),
                DefenseRoom = "Life Sciences 104",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            };

            context.Projects.AddRange(p1, p2, p3, p4, p5, p6);
            context.SaveChanges();

            // 3. Students
            var students = new List<Student>
            {
                new Student { FullName = "Tariq Mansour", StudentId = "ST-2022-8401", Email = "tariq.mansour@university.edu", Major = "AI & Data Science", Role = "Team Lead & ML Architect", GPA = 3.92, ProjectId = p1.Id, AvatarUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Layla Chen", StudentId = "ST-2022-8402", Email = "layla.chen@university.edu", Major = "Software Engineering", Role = "Full Stack Engineer", GPA = 3.85, ProjectId = p1.Id, AvatarUrl = "https://images.unsplash.com/photo-1517841905240-472988babdf9?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Zaid Al-Harbi", StudentId = "ST-2022-8403", Email = "zaid.alharbi@university.edu", Major = "Computer Engineering", Role = "Systems & Cloud Engineer", GPA = 3.78, ProjectId = p1.Id, AvatarUrl = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=100&auto=format&fit=crop&q=80" },

                new Student { FullName = "Farah Qasim", StudentId = "ST-2022-7911", Email = "farah.qasim@university.edu", Major = "Cybersecurity", Role = "Team Lead & Kernel Dev", GPA = 3.96, ProjectId = p2.Id, AvatarUrl = "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Dmitri Volkov", StudentId = "ST-2022-7912", Email = "dmitri.volkov@university.edu", Major = "Computer Networks", Role = "Protocol Engineer", GPA = 3.82, ProjectId = p2.Id, AvatarUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=100&auto=format&fit=crop&q=80" },

                new Student { FullName = "Sara Al-Otaibi", StudentId = "ST-2022-6320", Email = "sara.alotaibi@university.edu", Major = "Robotics & Embedded", Role = "Team Lead & VIO Specialist", GPA = 3.91, ProjectId = p3.Id, AvatarUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Lucas Romero", StudentId = "ST-2022-6321", Email = "lucas.romero@university.edu", Major = "Computer Engineering", Role = "Firmware & Mesh Hardware", GPA = 3.75, ProjectId = p3.Id, AvatarUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=100&auto=format&fit=crop&q=80" },

                new Student { FullName = "Yasmine Benali", StudentId = "ST-2022-5501", Email = "yasmine.benali@university.edu", Major = "Cloud Computing", Role = "Team Lead & K8s Dev", GPA = 3.88, ProjectId = p4.Id, AvatarUrl = "https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Arjun Mehta", StudentId = "ST-2022-5502", Email = "arjun.mehta@university.edu", Major = "Software Engineering", Role = "Backend & Metrics Engineer", GPA = 3.84, ProjectId = p4.Id, AvatarUrl = "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?w=100&auto=format&fit=crop&q=80" },

                new Student { FullName = "Maya Haddad", StudentId = "ST-2022-4110", Email = "maya.haddad@university.edu", Major = "Software Engineering", Role = "Lead Protocol Architect", GPA = 3.94, ProjectId = p5.Id, AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=100&auto=format&fit=crop&q=80" },
                new Student { FullName = "Nikhil Sharma", StudentId = "ST-2022-3890", Email = "nikhil.sharma@university.edu", Major = "Data Science", Role = "GNN Researcher", GPA = 3.89, ProjectId = p6.Id, AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100&auto=format&fit=crop&q=80" }
            };
            context.Students.AddRange(students);
            context.SaveChanges();

            // 4. Milestones
            var milestones = new List<Milestone>
            {
                new Milestone { ProjectId = p1.Id, Title = "Literature Review & Model Architecture", Description = "Benchmark Transformer vs 3D-CNN backbones on BraTS dataset.", DueDate = DateTime.UtcNow.AddMonths(-5), CompletedDate = DateTime.UtcNow.AddMonths(-5), Status = MilestoneStatus.Approved, WeightPercentage = 20, Feedback = "Excellent benchmark methodology." },
                new Milestone { ProjectId = p1.Id, Title = "Model Training & Multimodal Fusion", Description = "Achieve >92% AUC-ROC score across 4 validation folds.", DueDate = DateTime.UtcNow.AddMonths(-3), CompletedDate = DateTime.UtcNow.AddMonths(-3), Status = MilestoneStatus.Approved, WeightPercentage = 30, Feedback = "Exceptional results with cross-attention fusion." },
                new Milestone { ProjectId = p1.Id, Title = "Clinical Web PACS Integration", Description = "Build DICOM viewer and FHIR compliant REST APIs.", DueDate = DateTime.UtcNow.AddMonths(-1), CompletedDate = DateTime.UtcNow.AddMonths(-1), Status = MilestoneStatus.Approved, WeightPercentage = 30, Feedback = "Clean architecture and DICOM standard conformance." },
                new Milestone { ProjectId = p1.Id, Title = "Final Thesis & Defense Presentation", Description = "Comprehensive graduation dissertation and committee presentation.", DueDate = DateTime.UtcNow.AddDays(-14), CompletedDate = DateTime.UtcNow.AddDays(-14), Status = MilestoneStatus.Approved, WeightPercentage = 20, Feedback = "Distinguished project work." },

                new Milestone { ProjectId = p2.Id, Title = "Threat Modeling & eBPF Prototype", Description = "Map SCADA attacks (Stuxnet, Industroyer) and implement raw packet hook.", DueDate = DateTime.UtcNow.AddMonths(-4), CompletedDate = DateTime.UtcNow.AddMonths(-4), Status = MilestoneStatus.Approved, WeightPercentage = 25, Feedback = "Very thorough security analysis." },
                new Milestone { ProjectId = p2.Id, Title = "Zero-Trust Mesh & TPM 2.0 Attestation", Description = "Integrate hardware TPM keys and mutual TLS handshake.", DueDate = DateTime.UtcNow.AddMonths(-2), CompletedDate = DateTime.UtcNow.AddMonths(-2), Status = MilestoneStatus.Approved, WeightPercentage = 35, Feedback = "Solid cryptographic implementation." },
                new Milestone { ProjectId = p2.Id, Title = "Testbed Penetration Testing & Report", Description = "Simulate 12 attack vectors on live simulated PLC water rig.", DueDate = DateTime.UtcNow.AddDays(-2), CompletedDate = DateTime.UtcNow.AddDays(-2), Status = MilestoneStatus.Approved, WeightPercentage = 40, Feedback = "Ready for final defense committee." },

                new Milestone { ProjectId = p3.Id, Title = "LoRa/Wi-Fi Ad-Hoc Protocol Stack", Description = "Implement decentralized routing and heartbeat broadcast.", DueDate = DateTime.UtcNow.AddMonths(-3), CompletedDate = DateTime.UtcNow.AddMonths(-3), Status = MilestoneStatus.Approved, WeightPercentage = 30, Feedback = "Good throughput benchmark." },
                new Milestone { ProjectId = p3.Id, Title = "Distributed VIO SLAM & Point Cloud", Description = "Stereo camera real-time feature extraction and voxel merging.", DueDate = DateTime.UtcNow.AddDays(5), Status = MilestoneStatus.InProgress, WeightPercentage = 40, Feedback = "Pending field test calibration." },
                new Milestone { ProjectId = p3.Id, Title = "Live Obstacle Avoidance Flight Test", Description = "Indoor search & rescue mock trial in darkened facility.", DueDate = DateTime.UtcNow.AddDays(20), Status = MilestoneStatus.Pending, WeightPercentage = 30 }
            };
            context.Milestones.AddRange(milestones);

            // 5. Evaluations
            var evaluations = new List<Evaluation>
            {
                new Evaluation
                {
                    ProjectId = p1.Id,
                    SupervisorId = supervisors[0].Id,
                    EvaluatorName = "Prof. Elena Rostova",
                    EvaluatorRole = "Primary Supervisor",
                    PresentationScore = 19.5,
                    ImplementationScore = 39.0,
                    DocumentationScore = 19.0,
                    InnovationScore = 19.0,
                    Comments = "Groundbreaking research contribution with tangible healthcare impact. Code quality and testing coverage are exemplary.",
                    EvaluationDate = DateTime.UtcNow.AddDays(-14)
                },
                new Evaluation
                {
                    ProjectId = p1.Id,
                    SupervisorId = supervisors[1].Id,
                    EvaluatorName = "Dr. Marcus Vance",
                    EvaluatorRole = "Internal Committee Chair",
                    PresentationScore = 19.0,
                    ImplementationScore = 38.5,
                    DocumentationScore = 19.5,
                    InnovationScore = 19.5,
                    Comments = "Outstanding defense. Clear mastery of both machine learning mathematics and software architecture.",
                    EvaluationDate = DateTime.UtcNow.AddDays(-14)
                },
                new Evaluation
                {
                    ProjectId = p4.Id,
                    SupervisorId = supervisors[2].Id,
                    EvaluatorName = "Dr. Amina Al-Mansoor",
                    EvaluatorRole = "Primary Supervisor",
                    PresentationScore = 18.5,
                    ImplementationScore = 38.0,
                    DocumentationScore = 18.5,
                    InnovationScore = 19.0,
                    Comments = "Significant reduction in cloud carbon footprint validated on realistic distributed clusters. High quality engineering.",
                    EvaluationDate = DateTime.UtcNow.AddDays(-20)
                }
            };
            context.Evaluations.AddRange(evaluations);

            // 6. Announcements
            var announcements = new List<Announcement>
            {
                new Announcement
                {
                    Title = "Spring 2026 Final Defense Schedule & Committee Allocations Released",
                    Content = "The committee defense timetable for all graduating seniors is now officially scheduled. Please verify your assigned defense hall, time slot, and bring 3 bound copies of your project report.",
                    Category = "Defense",
                    Priority = "Urgent",
                    AuthorName = "Graduation Project Committee",
                    PublishedDate = DateTime.UtcNow.AddDays(-2),
                    ActionLabel = "View Defense Schedule",
                    ActionUrl = "/Defense"
                },
                new Announcement
                {
                    Title = "Mandatory Plagiarism & Code Originality Verification (Turnitin/Moss)",
                    Content = "All project teams must submit their final thesis documents to the Turnitin portal and push final git repositories before the cutoff. Similarity index must not exceed 15% excluding standard library references.",
                    Category = "Guidelines",
                    Priority = "Important",
                    AuthorName = "Dean of Academic Quality",
                    PublishedDate = DateTime.UtcNow.AddDays(-5),
                    ActionLabel = "Read Guidelines",
                    ActionUrl = "/Announcements"
                },
                new Announcement
                {
                    Title = "Annual University Innovation & Tech Showcase Registration Open",
                    Content = "Top-tier graduation projects will be selected for presentation at the Annual Engineering Expo in front of industry partners and venture scouts. High-impact awards and funding grants available.",
                    Category = "General",
                    Priority = "Normal",
                    AuthorName = "Industry Relations Office",
                    PublishedDate = DateTime.UtcNow.AddDays(-10),
                    ActionLabel = "Submit Project for Expo",
                    ActionUrl = "/Submit"
                }
            };
            context.Announcements.AddRange(announcements);

            context.SaveChanges();
        }
    }
}