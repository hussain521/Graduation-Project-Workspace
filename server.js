const express = require('express');
const cors = require('cors');
const path = require('path');
const store = require('./data/store');

const app = express();
const PORT = process.env.PORT || 5000;
const HOST = '0.0.0.0';

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Serve static UI assets
app.use(express.static(path.join(__dirname, 'public')));

// API Routes
app.get('/api/stats', (req, res) => {
  res.json(store.getStats());
});

app.get('/api/supervisors', (req, res) => {
  res.json(store.getSupervisors());
});

app.get('/api/projects', (req, res) => {
  const { search, track, status } = req.query;
  const projects = store.getProjects({ search, track, status });
  res.json(projects);
});

app.get('/api/projects/:id', (req, res) => {
  const project = store.getProjectById(req.params.id);
  if (!project) {
    return res.status(404).json({ error: 'Project not found' });
  }
  res.json(project);
});

app.post('/api/projects', (req, res) => {
  const { title, abstract, track, supervisorId, techStack, teamMembers } = req.body;
  if (!title || !abstract) {
    return res.status(400).json({ error: 'Title and abstract are required.' });
  }
  const created = store.addProject({
    title,
    abstract,
    track,
    supervisorId,
    techStack,
    teamMembers: Array.isArray(teamMembers) ? teamMembers : [
      {
        id: Date.now(),
        fullName: teamMembers?.fullName || 'Graduating Senior',
        studentId: teamMembers?.studentId || `ST-2022-${Math.floor(1000 + Math.random() * 9000)}`,
        role: 'Team Lead',
        major: 'Software Engineering'
      }
    ]
  });
  res.status(201).json(created);
});

app.post('/api/evaluations', (req, res) => {
  const { projectId, evaluatorName, evaluatorRole, presentationScore, implementationScore, documentationScore, innovationScore, comments } = req.body;
  if (!projectId) {
    return res.status(400).json({ error: 'ProjectId is required.' });
  }
  const result = store.addEvaluation({
    projectId,
    evaluatorName,
    evaluatorRole,
    presentationScore,
    implementationScore,
    documentationScore,
    innovationScore,
    comments
  });
  if (!result) {
    return res.status(404).json({ error: 'Project not found.' });
  }
  res.json({ success: true, ...result });
});

app.put('/api/projects/:projectId/milestones/:milestoneId', (req, res) => {
  const { status } = req.body;
  const updated = store.updateMilestoneStatus(req.params.projectId, req.params.milestoneId, status);
  if (!updated) {
    return res.status(404).json({ error: 'Milestone or Project not found.' });
  }
  res.json(updated);
});

app.get('/api/announcements', (req, res) => {
  res.json(store.getAnnouncements());
});

// HTML Page Routes
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

app.get('/projects', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'projects.html'));
});

app.get('/milestones', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'milestones.html'));
});

app.get('/defense', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'defense.html'));
});

app.get('/supervisors', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'supervisors.html'));
});

app.get('/api-docs', (req, res) => {
  res.sendFile(path.join(__dirname, 'public', 'api-docs.html'));
});

// Start server
app.listen(PORT, HOST, () => {
  console.log(`\n======================================================`);
  console.log(`🚀 NexusGrad Graduation Project Workspace is running!`);
  console.log(`🌐 Web URL: http://0.0.0.0:${PORT}`);
  console.log(`📡 REST API & Console: http://0.0.0.0:${PORT}/api-docs`);
  console.log(`======================================================\n`);
});