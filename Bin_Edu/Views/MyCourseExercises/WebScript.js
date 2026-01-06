let submittedFile = null;

function submitExercise() {
  const form = document.getElementById("exerciseForm");
  const fileInput = document.getElementById("exerciseFile");
  const file = fileInput.files[0];

  // Validate form
  if (!form.checkValidity()) {
    form.reportValidity();
    return;
  }

  // Validate file size (10MB)
  if (file && file.size > 10 * 1024 * 1024) {
    alert("File size exceeds 10MB limit.");
    return;
  }

  // Store submitted file info
  submittedFile = {
    name: file.name,
    size: file.size,
    type: file.type,
    lastModified: new Date(file.lastModified),
    file: file,
  };

  // In a real application, you would send this data to a server
  console.log("Submitting exercise file:", submittedFile);

  // Close modal
  const modal = bootstrap.Modal.getInstance(
    document.getElementById("submitExerciseModal")
  );
  modal.hide();

  // Show success message
  const successAlert = document.getElementById("successAlert");
  successAlert.style.display = "block";

  // Reset form
  form.reset();

  // Hide success message after 5 seconds
  setTimeout(() => {
    successAlert.style.display = "none";
  }, 5000);
}

function formatFileSize(bytes) {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
}

// Update view modal when opened
document
  .getElementById("viewSubmissionModal")
  .addEventListener("show.bs.modal", function () {
    const detailsDiv = document.getElementById("submissionDetails");

    if (submittedFile) {
      detailsDiv.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title mb-3">File Information</h6>
                            <table class="table table-sm">
                                <tbody>
                                    <tr>
                                        <th style="width: 150px;">File Name:</th>
                                        <td>${submittedFile.name}</td>
                                    </tr>
                                    <tr>
                                        <th>File Size:</th>
                                        <td>${formatFileSize(
                                          submittedFile.size
                                        )}</td>
                                    </tr>
                                    <tr>
                                        <th>File Type:</th>
                                        <td>${
                                          submittedFile.type || "Unknown"
                                        }</td>
                                    </tr>
                                    <tr>
                                        <th>Submitted:</th>
                                        <td>${submittedFile.lastModified.toLocaleString()}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="mt-3">
                                <button class="btn btn-sm btn-primary" onclick="downloadFile()">
                                    <i class="bi bi-download"></i> Download File
                                </button>
                            </div>
                        </div>
                    </div>
                `;
    } else {
      detailsDiv.innerHTML = `
                    <div class="alert alert-info">
                        No file has been submitted yet.
                    </div>
                `;
    }
  });

function downloadFile() {
  if (submittedFile && submittedFile.file) {
    const url = URL.createObjectURL(submittedFile.file);
    const a = document.createElement("a");
    a.href = url;
    a.download = submittedFile.name;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
}

// Reset form when modal is closed
document
  .getElementById("submitExerciseModal")
  .addEventListener("hidden.bs.modal", function () {
    document.getElementById("exerciseForm").reset();
  });
