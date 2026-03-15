// admin-reports.js - Reports page functionality with Chart.js

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Activity Filter
    const activityFilter = document.getElementById('activityFilter');
    const activityItems = document.querySelectorAll('.activity-item');

    activityFilter.addEventListener('change', function() {
        const filterValue = this.value;

        activityItems.forEach(item => {
            if (filterValue === 'all') {
                item.style.display = '';
            } else {
                const iconClass = item.querySelector('.activity-icon i').className;
                let itemType = '';

                if (iconClass.includes('person-plus')) itemType = 'enrollment';
                else if (iconClass.includes('file-earmark-text')) itemType = 'submission';
                else if (iconClass.includes('megaphone')) itemType = 'announcement';
                else if (iconClass.includes('box-arrow-in-right')) itemType = 'login';
                else if (iconClass.includes('check-circle')) itemType = 'submission';

                item.style.display = (itemType === filterValue) ? '' : 'none';
            }
        });
    });

    // Report Period Change
    const reportPeriod = document.getElementById('reportPeriod');
    reportPeriod.addEventListener('change', function() {
        // In a real app, this would fetch new data based on the period
        alert(`Loading data for: ${this.options[this.selectedIndex].text}`);
        // For demo, we'll just update the charts with new mock data
        updateCharts();
    });

    // Export Report Function
    window.exportReport = function() {
        alert('Export functionality would generate a PDF/Excel report with current data.');
    };

    // Initialize Charts
    let activityChart, userDistributionChart, departmentChart, performanceChart;

    function initCharts() {
        // Activity Trends Chart
        const activityCtx = document.getElementById('activityChart').getContext('2d');
        activityChart = new Chart(activityCtx, {
            type: 'line',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Active Users',
                    data: [245, 289, 312, 298, 334, 267, 198],
                    borderColor: '#1E40AF',
                    backgroundColor: 'rgba(30, 64, 175, 0.1)',
                    tension: 0.4,
                    fill: true
                }, {
                    label: 'Course Views',
                    data: [456, 523, 587, 612, 678, 534, 423],
                    borderColor: '#059669',
                    backgroundColor: 'rgba(5, 150, 105, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        // User Distribution Chart
        const userDistCtx = document.getElementById('userDistributionChart').getContext('2d');
        userDistributionChart = new Chart(userDistCtx, {
            type: 'doughnut',
            data: {
                labels: ['Students', 'Instructors', 'Admins'],
                datasets: [{
                    data: [1056, 156, 35],
                    backgroundColor: ['#0EA5E9', '#059669', '#7C3AED'],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                    }
                }
            }
        });

        // Department Chart
        const deptCtx = document.getElementById('departmentChart').getContext('2d');
        departmentChart = new Chart(deptCtx, {
            type: 'bar',
            data: {
                labels: ['CS', 'EE', 'MT', 'HU', 'Other'],
                datasets: [{
                    label: 'Enrollments',
                    data: [423, 234, 198, 156, 89],
                    backgroundColor: '#1E40AF',
                    borderRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        // Performance Chart
        const perfCtx = document.getElementById('performanceChart').getContext('2d');
        performanceChart = new Chart(perfCtx, {
            type: 'radar',
            data: {
                labels: ['Response Time', 'Uptime', 'Throughput', 'Error Rate', 'User Satisfaction'],
                datasets: [{
                    label: 'Current',
                    data: [85, 99.9, 92, 95, 88],
                    borderColor: '#059669',
                    backgroundColor: 'rgba(5, 150, 105, 0.2)',
                    pointBackgroundColor: '#059669'
                }, {
                    label: 'Target',
                    data: [90, 99.9, 95, 98, 90],
                    borderColor: '#7C3AED',
                    backgroundColor: 'rgba(124, 58, 237, 0.2)',
                    pointBackgroundColor: '#7C3AED'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                    }
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 100
                    }
                }
            }
        });
    }

    function updateCharts() {
        // Simulate updating charts with new data based on period
        const periods = {
            week: {
                activity: [245, 289, 312, 298, 334, 267, 198],
                views: [456, 523, 587, 612, 678, 534, 423]
            },
            month: {
                activity: [1200, 1350, 1420, 1380, 1520, 1290, 1100],
                views: [2400, 2650, 2780, 2690, 2890, 2450, 2100]
            },
            quarter: {
                activity: [4500, 4800, 5200, 5100, 5400, 5300, 4900, 4600, 4300, 4100, 3900, 3700],
                views: [9200, 9800, 10500, 10200, 10800, 10600, 9800, 9200, 8600, 8200, 7800, 7400]
            },
            year: {
                activity: [15000, 16500, 17200, 16800, 18200, 17900, 17500, 16200, 15800, 15200, 14800, 14200],
                views: [31000, 33500, 35200, 34800, 37200, 36800, 35500, 33200, 32800, 31200, 30800, 29400]
            }
        };

        const period = reportPeriod.value;
        const data = periods[period];

        if (activityChart) {
            activityChart.data.datasets[0].data = data.activity;
            activityChart.data.datasets[1].data = data.views;
            activityChart.update();
        }
    }

    // Initialize charts on page load
    initCharts();

    // Animate stat cards on load
    const statCards = document.querySelectorAll('.card-body h3');
    statCards.forEach((card, index) => {
        setTimeout(() => {
            card.style.animation = 'fadeInUp 0.6s ease-out';
        }, index * 100);
    });
});